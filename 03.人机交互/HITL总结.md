# 人机交互总结

## 1. 什么是人机交互

在 Agent Framework 的 Workflow 里，人机交互（Human-in-the-loop, HITL）指的是：

- 工作流运行到某一步时，不能自动继续
- 需要外部系统或人工提供一个响应
- Workflow 暂停并抛出 `RequestInfoEvent`
- 宿主程序接住这个事件，收集人工输入，再把响应送回工作流

这套机制既可以处理我们自己定义的人工请求，也可以处理 Agent 调用函数时的人工审批。

## 2. 核心角色

### `RequestPort`

`RequestPort` 是“对外发起请求”的端口。

例如：

```csharp
var numberRequestPort = RequestPort.Create<NumberSignal, int>("GuessNumber");
```

这句代码的含义是：

- 请求类型是 `NumberSignal`
- 响应类型是 `int`
- 端口名称是 `GuessNumber`

其中 `"GuessNumber"` 很重要，它是后续区分请求来源的稳定标识。

### `RequestInfoEvent`

当 Workflow 运行到需要外部响应的地方时，会发出 `RequestInfoEvent`。

宿主程序通常会在：

```csharp
await foreach (var evt in run.WatchStreamAsync())
{
    if (evt is RequestInfoEvent requestInfoEvent)
    {
        // 在这里处理人工输入
    }
}
```

拿到事件后，需要做两件事：

- 判断这是什么请求
- 调用 `SendResponseAsync(...)` 把响应送回去

### `Executor`

`Executor` 负责业务逻辑本身。

在当前 Demo 里，`JudgeExecutor` 的职责是：

- 接收用户猜的数字
- 判断大小
- 如果没猜中，就再次发起请求
- 如果猜中，就输出结果

也就是说：

- `RequestPort` 负责“向外要输入”
- `Executor` 负责“拿到输入后做判断”

## 3. 一个完整的运行链路

以猜数字 Demo 为例：

1. Workflow 从 `RequestPort` 开始
2. 宿主收到 `RequestInfoEvent`
3. 用户输入一个数字
4. 宿主调用 `SendResponseAsync(...)`
5. `JudgeExecutor` 收到数字并判断
6. 如果没猜中，再次把提示信号送回 `RequestPort`
7. 如果猜中，输出最终结果

所以它不是“执行器自己去读控制台”，而是：

- Workflow 发请求
- 宿主统一处理请求
- 宿主把结果回填

## 4. 为什么不能把执行器类型放进 `RequestPort.Create`

错误示例：

```csharp
var numberRequestPort = RequestPort.Create<NumberSignalExecutor, int>("GetNumber");
```

原因是：

- `RequestPort.Create<TRequest, TResponse>()` 的第一个泛型参数是“请求数据类型”
- 不是执行器类型

正确思路是：

```csharp
var numberRequestPort = RequestPort.Create<NumberSignal, int>("GuessNumber");
```

然后再用工作流边把它和执行器接起来：

```csharp
var workflow = new WorkflowBuilder(numberRequestPort)
    .AddEdge(numberRequestPort, judgeExecutor)
    .AddEdge(judgeExecutor, numberRequestPort)
    .WithOutputFrom(judgeExecutor)
    .Build();
```

## 5. `RequestInfoEvent` 怎么区分不同请求

### 按请求内容区分

如果一个端口里会发出不同语义的请求，可以看请求体：

```csharp
requestInfoEvent.Request.Request
```

例如：

- `NumberSignal.Init`
- `NumberSignal.Above`
- `NumberSignal.Below`

### 按端口名称区分

如果多个请求的元数据类型一样，更推荐按端口名区分：

```csharp
var portId = requestInfoEvent.Request.PortInfo.PortId;
```

这个 `PortId` 就是创建 `RequestPort` 时传入的名字，比如 `"GuessNumber"`。

推荐规则：

- 区分“是哪类请求”时，用 `PortInfo.PortId`
- 区分“这一次具体是哪次请求”时，用 `RequestId`
- 区分“请求里带了什么业务数据”时，用 `Request.Request`

## 6. 建议有一个统一的人机交互处理组件

在工程化场景里，不建议把人工处理逻辑散落在每个业务执行器中。

更推荐单独放一个统一处理组件，例如：

```csharp
await foreach (var evt in run.WatchStreamAsync())
{
    switch (evt)
    {
        case RequestInfoEvent requestInfoEvent:
            await humanInteractionHandler.HandleAsync(run, requestInfoEvent);
            break;

        case WorkflowOutputEvent outputEvent:
            Console.WriteLine(outputEvent.Data);
            break;
    }
}
```

这个组件的职责是：

- 监听所有 `RequestInfoEvent`
- 按 `PortId` 或请求类型分发
- 和控制台、Web 页面、审批系统等 UI 打交道
- 最终统一调用 `SendResponseAsync(...)`

这样做的好处：

- Workflow 只关心业务
- UI 逻辑集中
- 后续从控制台切到 Web/API 时改动更小
- 便于统一做审批、审计、日志

## 7. Agent 调用函数时需要人工审批怎么办

这也是人机交互的一种。

当 Agent 调用“需要审批”的函数工具时，Workflow 一样会暂停，并抛出 `RequestInfoEvent`。

只是这时候事件里携带的不是我们自定义的请求类型，而通常是：

- `ToolApprovalRequestContent`
- 或 `FunctionApprovalRequestContent`

处理思路不变：

1. 统一的人机交互组件收到 `RequestInfoEvent`
2. 判断这是“普通人工请求”还是“工具审批请求”
3. 如果是工具审批，就展示函数名、参数、审批说明
4. 用户选择同意或拒绝
5. 宿主把审批结果作为 response 送回去

伪代码如下：

```csharp
public async Task HandleAsync(StreamingRun run, RequestInfoEvent evt)
{
    var request = evt.Request;

    if (request.Request is FunctionApprovalRequestContent approval)
    {
        var approved = ReadApproval();
        await run.SendResponseAsync(request.CreateResponse(approved));
        return;
    }

    if (request.PortInfo.PortId == "GuessNumber")
    {
        var guess = ReadInt();
        await run.SendResponseAsync(request.CreateResponse(guess));
        return;
    }
}
```

结论就是：

- 普通人工请求和工具审批，入口都是 `RequestInfoEvent`
- 只是 payload 类型不同
- 最终回填方式仍然是 `CreateResponse(...)` + `SendResponseAsync(...)`

## 8. 当前 Demo 的设计要点

`03.人机交互` 当前 Demo 适合用来理解以下概念：

- `RequestPort` 不是执行器
- `RequestPort` 需要通过工作流边连接到执行器
- `RequestInfoEvent` 是宿主处理人工输入的入口
- 人工输入回填后，Workflow 会继续运行
- 人工审批和普通 HITL 请求本质上是同一套机制

## 9. 实战建议

- 给每个 `RequestPort` 一个清晰、稳定、唯一的名字
- 不要把 UI 输入逻辑直接写进业务执行器
- 用统一的人机交互处理组件接管所有 `RequestInfoEvent`
- 如果有多个审批或请求类型，优先按 `PortId` 分发
- 如果存在并发请求，注意用 `RequestId` 关联响应
- 做 Web 化时，把控制台输入替换成前端表单或审批页面即可

## 10. 参考文档

- Human-in-the-loop:
  https://learn.microsoft.com/zh-cn/agent-framework/workflows/human-in-the-loop?pivots=programming-language-csharp
- Request and Response:
  https://learn.microsoft.com/en-us/agent-framework/user-guide/workflows/request-and-response
- Workflow Events:
  https://learn.microsoft.com/en-us/agent-framework/workflows/events
- Tool approval:
  https://learn.microsoft.com/en-us/agent-framework/agents/tools/tool-approval
