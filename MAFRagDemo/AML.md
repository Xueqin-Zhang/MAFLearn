是一种XML方言，遵循simple /Item/Relationships/Item/Relationships重复模式来描述Item配置。客户端向Aras Innovator服务器提交AML文档，并 收到AML文档返回。 AML文档包含数据(**Items**)、结构(**Relationships**，即分层**Item**)和逻辑(在Item上执行某些业务逻辑的动作)。AML文档中的 每个Item都有一个**action**属性，这是在Item上执行业务逻辑的Aras Innovator Method的名称。Aras Innovator服务器对AML 文档的解释类似于脚本语言。AML文档通常被称为AML脚本。  

## 标记
### <Item>标记
`<Item>`标签定义了一个**Item**实例。XML是区分大小写的。标签有三个主要的属性来定义`Item`实例

1. `id` Item的唯一id
2. `type`项目的`ItemType`名称
3. `action`应用于项目的方法名称

### 内置Action
+ `add`将Item添加为ItemType的实例
+ `update`更新Item，**Item必须被锁定**、如果Item是可换版的，并且是被锁定以来第一次更新，则将更新应用于新版本的Item进行版本化，除非指定了`version='0'`属性，该属性禁用了版本化。
+ `purge`删除该物品的版本。
+ `delete`删除该Item的所有版本。`purge`和`delete`对于不可换版的项目是相同的。
+ `get`用于查询数据库的AML Item
+ `getItemConfig`与get操作没有什么不同。此是通过SQL调用和AML结果逻辑进行了优化。性能是通过限制`Innovator GetItem`中可用的功能（服务器事件或对子集Items的访问检查）来获得
+ `edit`锁定、更新和解锁Item
+ `create`如果Item存在，则充当获取，否则添加
+ `merge`如果条目存在则编辑，否则添加
+ `lock、unlock`对Item锁定和解锁
+ `version`创建一个Item的新一代，清楚原始的Item的`locked_by_id`，并设置新一代的`locked_by_id`。服务器事件按`onBeforeVersion、onAfterVersion、onBeforeUpdate、onAfterUpdate`如果该项目不可版本化，则抛出异常

### <Relationships>标签
项目可以与其他项目有关系。标签是一个容器标签，它保存了一组关系项。再次注意大写的 Relationships。标签没有属性，因为它是一个容器。有了关系，就可以根据需要将Item的配置描述到任何深度。 

AML是Aras Innovator再封装`xml`语法格式

### <property>属性
 Item的属性是直接在标签下面嵌套的标签。Property name是标签的名称。例如，一个“PartItemType”可 能有属性:item_number、description和cost，它们也是AML中的标签名称。**属性名是小写的**，所以属性标签名也 是小写的。 

## 属性
属性用于定义Item的数据。属性是Item或Property的元数据。它们被用来控制服务器逻辑和方法。可以把属性 想象成命令行开关，或者函数的参数。  

### Item属性
| 属性 | 类型 | 使用 |
| --- | --- | --- |
| type | String | Item是其实例的ItemType名称 |
| id | String | 实例的唯一ID值 |
| where | String | 用于代替id属性，为搜索条件指定`where`子句。包含表明和列明 |
| action | String | 应用于Item的方法(或内置动作方法)的名称 |
| doGetItem | Boolean | 如果为0，执行action操作后，不对Item执行最终get操作。默认值1 |
| **与action="get"连用** | | |
| select | String | 要返回的属性名(列名)用`,`分隔，即SQL中的Select |
| orderBy | String | 排序，相当于SQL中的order by |
| page | Integer | 结果集的页码 |
| pagesize | Integer | 返回条数 |
| maxRecords | Integer | 定义数据库中搜索的最大项目 |
| levels | Integer | 搜索Item的深度 |
| serverEvents | Boolean | 为0，则禁用服务器事件提高性能。默认是1 |
| isCriteria | Boolean | 为0，响应中包含Item配置的嵌套结构，不能作为搜索条件。默认是1，它是使用请求中的嵌套接口作为搜索条件 |
| related_expand | Boolean | 如果为0则不展开关系项目的`related_id`属性已包含的香港项目。为1时，返回的是他的ID |
| language | String | 已`,`号分割，返回指定的多语言属性值（如果存在） |
| **与action="update" | "edit"一起使用** | | |
| version | Boolean | 为0不更新项目版本。默认值1，更新项目版本（可换版项目） |
| serverEvents | Boolean | 为0禁用服务器事件，默认是至今用`update`事件，`lock`事件在使用`edit`执行。 |
| type | String | 实例ItemType名称 |
| keyed_name | String | 引用项目的`keyed_name`属性 |
| condition | String | 条件查询，支持`sql`的所有查询条件 |


#### 查询操作符 `condition`
+ `eq` 相当于`==`相等， `<name condition="eq">熊小</name>`
+ `ne` 不能于，获取没有这个结果的其余选项
+ `ge`大于等于，获取大于等于这个结果的数据
+ `le` 小于等于，获取小于等于这个结果的数据
+ `gt`  属性大于另一个值
+ `like / not like` 包括%或*通配符符号
+ `between / not between` 对范围使用和关键字
+ `in，not in` 值以逗号分隔，包含多个结果
+ `is null / is not null` 是null或不是null





配合[Nash](http://shsl1/OOTB1209/Client/X-salt=1_12.0.0.24090-X/scripts/nash.aspx)进行查询

<!-- 这是一张图片，ocr 内容为：PASSWORD: TRAIN STU07 USER. DATABASE: ADMIN MD5 PASSWORD HASH ALGORITHM: 1,选择数据库登录进行之后操作. SHA256 TIMEZONE: EASTERN STANDARD TIME CULTURE: EN-US-ENGLISH(UNITED STATES) LOGIN ACTION: SERVER: HTTP://SHS/1/OOTB1209/SERVER/INNOVATORSERVER.ASPX APPLYAML XML: <AML> SUBMIT LTEM TYPE "PART"ACTION"GET"> <CLASSIFICATION>ASSEMBLY</CLASSIFICATION> CLEAR <ITEM NUMBER>SD-1006</ITEM NUMBER> 2提交编写的AML </LTEM> <IAML> 执行时间 RUN TIME,SEC: 0,021 RESULT: READ ICSOAP-ENVELOPE XNINS SOAP SOAP ENV-THTP//SCHEMAS XMLSOAPLENVELOPER><SOAP ENVBODY>:RESULD-TITEM SHOWXML <CLASSIFICATION>ASSEMBLY</CLASSIFICATION><CONFIQ ID KEVED NAME-"SD-1006" 另页面打开XML,格式 D NAME"LNNOVATOR ADMIN" TYPE三'PART'>45C338D12EB945188E782DF2D1674DDO</CONFIQ ID><CREATED BY ID KEYED NA LYPE-'USER'-30B991F9272/4FA3829655F50C994/2E JN72E J 经过整理 <CURRENT STATE NAME-"PRELIMINARY"KEYED NAME-"PRELIMINARY" TYPE-" TYPE CYCLE STATE*72A2322564FE4193933CFB5339487A06</CURRENT STATE<GENERATION> ICHAS CHANGE PENDING>0</HAS CHANGE PENDING>ID KEVED NAME-"SD-1006" LTYPE:-PAR*>45C338D12EB945188E7B2DFAD167+DDO<TD>>: CURENT-1SIS CURENT><IS RELEASED>OSIS {ELEASED>  KEYED NAME>SD-100GC/KEYED NAME>ZMALOR REVCACIMALMALOR REY>EMAKE,BUY>MAKE(MAKE,BUY><MODRRED,DY 10 PARTYPERPERMISSION" <STATE>PRELIMINARY</STATE><UNIT>EA</UNIT><ITEM NUMBER>SD-1006</TEM NUMBER -->
![](https://cdn.nlark.com/yuque/0/2023/png/969040/1677132741134-66ddd582-61eb-4a5e-aa7f-7a16310ccb46.png)

#### 基本查询
```xml
<AML>
  <Item type="part" action="get">
    <classification>Assembly</classification>
    <item_number>sd-1006</item_number>
  </Item>
</AML>
```

<!-- 这是一张图片，ocr 内容为：<SOAP-ENV:ENVELOPE> <SOAP-ENV:BODY> -<RESULT> - CITEN S' PART " IST ' AFIACOLACORAZBABABAABAAEZODB63BOBABB" ID 三" 45C338D12EB945188E7B2DFZDL674DDO <CLASSIFICATION>ASSEMBLY</CLASSIFICATION> <CONFIG-ID KEYED NAME :" SD-1006" TYPE ;" PART'>45C338D12EB945188E7B2DF2DF2D1674DDO</CONFIG_ID> <TEATED.5Y,JO KEYED LDAME ;" INNOVATOR ADMIN " IPE    USET  7308991F927374FA38236555555 5REATED,DY,DY <CREATED_ON>2023-02-23T00:28:52</CREATED_ON> <GENERATION>1</GENERATION> <HAS_CHANGE_PENDING>0</HAS_CHANGE_PENDING> <ID KEYED-NAME :" SD-1006 " TYPE -" PART" >45C338D12EB945188E7B2DF2D1674DD0</ID> <IS_CURRENT>1</IS_CURRENT> <IS_RELEASED>0</IS_RELEASED> <KEYED_NAME>SD-1006</KEYED_NAME> <MAJOR_REV>A</MAJOR_REV> <MAKE_BUY>MAKE</MAKE_BUY> -USER ">30B991F927274FA3829655F50C99472E</MODIFIED_BY_ID> <MODIFIED_B FIED_BY_ID KEYED_NAME "INNOVATOR ADMIN"""" TYPE " US ON>2023-02-23T00:28:52</MODIFIED_ON> <MODIFIED_ON>20 VERSION>0</NEW_VERSION> KNEW _LOCKABLE> BLE>0</ /NOT_ <NOT LOCKABLE ID KEYED AME ERMISSION PRELIMINARY <STATE> <UNIT>EA</UNIT> <ITEM_NUMBER>SD-1006< 06</ITEM NUMBER> <ITEMTYPE>4F1AC04A2B4841 F3ABA4E20DB63808A88</ITEMTYPE> </ITEM> </RESULT> </SOAP-ENV:BODY> </SOAP-ENV:ENVELOPE> -->
![](https://cdn.nlark.com/yuque/0/2023/png/969040/1677132543887-9dbe91d0-4531-4fef-9adf-d0fd16570fdf.png)

#### 关联查询
查询此对象关联的BOM的类对象

```xml
<AML>
  <Item type="Part" action="get">
    <item_number>sd-1002</item_number>
    <Relationships>
      <Item type="Part BOM" action="get">
        <related_id>
          <Item type="Part" action="get">
            <item_number>sd-10021</item_number>
          </Item>
        </related_id>
        <source_id>
          <Item type="Part" action="get">
          </Item>
        </source_id>
      </Item>
    </Relationships>
  </Item>
</AML>
```

#### BOM查询
每个`BOM`有个`source_id`和`related_id`代表父阶和子阶

```xml
<AML>
  <Item type="Part BOM" action="get">
    <related_id>
      <Item type="Part" action="get">
        <item_number>sd-10021</item_number>
      </Item>
    </related_id>
    <source_id>
      <Item type="Part" action="get">
      </Item>
    </source_id>
  </Item>
</AML>
```

### Item属性
1. `type`：用于指定 Item 的 ItemType。在上面的例子中，我们描述了一个 Part Item。
2. `id`：用于指定我们描述的数据库中的哪个 Part Item。在数据库中定位项的一种方法是提  
供唯一标识符。
3. `action`：用于指示将此 AML 发送到服务器时要执行的操作。Aras Innovator 有一套内置  
的动作。
+ `where` 用于标识项目（例如编辑、升级、删除）而不是id属性
+ `select` 选择要返回的字段
+ `orderBy` 对返回的结果进行排序
+ `page` 分页页码
+ `pageSize` 每页大小
+ `levels`	要查询的深度
+ `version` 版本
+ `idlist` Item的id，用`,`分割

```xml
<AML>
  <Item type="Part" action="get" 
    select="item_number,name" orderBy="item_number " page="1" pageSize="1"
    where="Part.item_number='sd-1002'"
  ></Item>
</AML>
```

<!-- 这是一张图片，ocr 内容为：<SOAP-ENV:ENVELOPE> <SOAP-ENV:BODY> <RESULT> 4F1AC04A2B484F3ABA4E20DB63808A88"ID -"AEEAEEAE -"AEEAE721A7954BF9BAF07ACECB6F3C06"> TYPEID - PART <ITEM TYPE IPART "AEEAE721A7954BF9BAF07ACECB6F3C06</ID> SD-1002"TYPE <ID KEYED_NAME <NAME>N2<NAME> <ITEM_NUMBER>SD-1002</ITEM_NUMBER> <ITEMTYPE>4F1AC04A2B484F3ABA4E20DB63808A88</ITEMTYPE> </ITEM> </RESULT> <SOAP-ENV:BODY> <SOAP-ENV:ENVELOPE> -->
![](https://cdn.nlark.com/yuque/0/2023/png/969040/1677133741282-3e7870fd-8114-49e2-a8ff-dc7c91cab57f.png)

查询的结果经过`select`精简

### 保存查询 Saving Queries
### Item类型
Item属性中`type`可以执行类型



#### 查询get
上面介绍过

#### 递归查询 GetItemRepeatConfig
```xml
<AML>
<Item type="Part" action="GetItemRepeatConfig" select="item_number,name" 
  id="01042A58E8F74A81AFFB8DA652B7288C">
  <Relationships>
     <Item type="Part BOM" repeatTimes="2" repeatProp="related_id" select="related_id,quantity">
     </Item>
  </Relationships>
    </Item>
</AML>
```

#### 添加add
```xml
<AML>
  <Item type="Part" action="add">
       <item_number>2134-9099</item_number>
       <name>Paper Sensor</name>
       <description>Front feed sensor</description>
       <make_buy>Buy</make_buy>
       <classification>Component</classification>
   </Item>
</AML>
```

#### 删除delete
```xml
<AML>
  <Item type="Part" action="delete"
    where="[part].state='Preliminary'">
  </Item>
</AML>
```

#### 修改edit
```xml
<AML>
  <Item type="Part" action="edit" where="[part].classification='Assembly’">
        <make_buy>Make</make_buy>
  </Item>
</AML>
```

#### 生命周期promoteItem
```xml
<AML>
  <Item type="Part" action="promoteItem" where="[part].item_number='5063-1257'">
   <state>Released</state>
  </Item>
</AML>
```

