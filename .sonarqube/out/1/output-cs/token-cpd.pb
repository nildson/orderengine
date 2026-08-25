ù7
EC:\projects\orderengine\OrderEngine.Application\ValidationBehavior.cs
	namespace 	
OrderEngine
 
. 
Application !
;! "
public 
sealed 
class 
ValidationBehavior &
<& '
TRequest' /
,/ 0
	TResponse1 :
>: ;
:< =
IPipelineBehavior> O
<O P
TRequestP X
,X Y
	TResponseZ c
>c d
where 	
TRequest
 
: 
notnull 
{		 
private

 
readonly

 
IEnumerable

  
<

  !

IValidator

! +
<

+ ,
TRequest

, 4
>

4 5
>

5 6
_validators

7 B
;

B C
public 

ValidationBehavior 
( 
IEnumerable )
<) *

IValidator* 4
<4 5
TRequest5 =
>= >
>> ?

validators@ J
)J K
{ 
_validators 
= 

validators  
;  !
} 
public 

async 
Task 
< 
	TResponse 
>  
Handle! '
(' (
TRequest( 0
request1 8
,8 9"
RequestHandlerDelegate: P
<P Q
	TResponseQ Z
>Z [
next\ `
,` a
CancellationTokenb s
cancellationToken	t Ö
)
Ö Ü
{ 
if 

( 
_validators 
. 
Any 
( 
) 
) 
{ 	
var 
context 
= 
new 
ValidationContext /
</ 0
TRequest0 8
>8 9
(9 :
request: A
)A B
;B C
var 
failures 
= 
_validators &
. 

SelectMany 
( 
	validator %
=>& (
	validator) 2
.2 3
Validate3 ;
(; <
context< C
)C D
.D E
ErrorsE K
)K L
. 
Where 
( 
error 
=> 
error  %
is& (
not) ,
null- 1
)1 2
. 
ToList 
( 
) 
; 
if 
( 
failures 
. 
Count 
!= !
$num" #
)# $
{ 
throw 
new 
ValidationException -
(- .
failures. 6
)6 7
;7 8
} 
} 	
return!! 
await!! 
next!! 
(!! 
)!! 
;!! 
}"" 
}## 
public%% 
sealed%% 
class%% '
CreateOrderCommandValidator%% /
:%%0 1
AbstractValidator%%2 C
<%%C D
CreateOrderCommand%%D V
>%%V W
{&& 
public'' 
'
CreateOrderCommandValidator'' &
(''& '
)''' (
{(( 
RuleFor)) 
()) 
x)) 
=>)) 
x)) 
.)) 

CustomerId)) !
)))! "
.** 
NotEmpty** 
(** 
)** 
.++ 
WithMessage++ 
(++ 
$str++ 2
)++2 3
;++3 4
RuleFor-- 
(-- 
x-- 
=>-- 
x-- 
.-- 
Items-- 
)-- 
... 
NotNull.. 
(.. 
).. 
.// 
WithMessage// 
(// 
$str// .
)//. /
;/// 0
RuleFor11 
(11 
x11 
=>11 
x11 
.11 
Items11 
)11 
.22 
Must22 
(22 
items22 
=>22 
items22  
is22! #
not22$ '
null22( ,
&&22- /
items220 5
.225 6
Any226 9
(229 :
)22: ;
)22; <
.33 
WithMessage33 
(33 
$str33 C
)33C D
;33D E
RuleForEach55 
(55 
x55 
=>55 
x55 
.55 
Items55  
)55  !
.66 

ChildRules66 
(66 
item66 
=>66 
{77 
item88 
.88 
RuleFor88 
(88 
x88 
=>88 !
x88" #
.88# $
	ProductId88$ -
)88- .
.99 
NotEmpty99 
(99 
)99 
.:: 
WithMessage::  
(::  !
$str::! 9
)::9 :
;::: ;
item<< 
.<< 
RuleFor<< 
(<< 
x<< 
=><< !
x<<" #
.<<# $
ProductName<<$ /
)<</ 0
.== 
NotEmpty== 
(== 
)== 
.>> 
WithMessage>>  
(>>  !
$str>>! ;
)>>; <
;>>< =
item@@ 
.@@ 
RuleFor@@ 
(@@ 
x@@ 
=>@@ !
x@@" #
.@@# $
Quantity@@$ ,
)@@, -
.AA 
GreaterThanAA  
(AA  !
$numAA! "
)AA" #
.BB 
WithMessageBB  
(BB  !
$strBB! F
)BBF G
;BBG H
itemDD 
.DD 
RuleForDD 
(DD 
xDD 
=>DD !
xDD" #
.DD# $
	UnitPriceDD$ -
)DD- .
.EE  
GreaterThanOrEqualToEE )
(EE) *
$numEE* +
)EE+ ,
.FF 
WithMessageFF  
(FF  !
$strFF! A
)FFA B
;FFB C
}GG 
)GG 
;GG 
}HH 
}II 
publicKK 
sealedKK 
classKK -
!UpdateOrderStatusCommandValidatorKK 5
:KK6 7
AbstractValidatorKK8 I
<KKI J$
UpdateOrderStatusCommandKKJ b
>KKb c
{LL 
publicMM 
-
!UpdateOrderStatusCommandValidatorMM ,
(MM, -
)MM- .
{NN 
RuleForOO 
(OO 
xOO 
=>OO 
xOO 
.OO 
IdOO 
)OO 
.PP 
NotEmptyPP 
(PP 
)PP 
.QQ 
WithMessageQQ 
(QQ 
$strQQ /
)QQ/ 0
;QQ0 1
RuleForSS 
(SS 
xSS 
=>SS 
xSS 
.SS 
StatusSS 
)SS 
.TT 
IsInEnumTT 
(TT 
)TT 
.UU 
WithMessageUU 
(UU 
$strUU ?
)UU? @
;UU@ A
}VV 
}WW ëw
AC:\projects\orderengine\OrderEngine.Application\OrderContracts.cs
	namespace 	
OrderEngine
 
. 
Application !
;! "
public 
record "
CreateOrderItemRequest $
($ %
string% +
	ProductId, 5
,5 6
string7 =
ProductName> I
,I J
intK N
QuantityO W
,W X
decimalY `
	UnitPricea j
)j k
;k l
public 
record 
CreateOrderRequest  
(  !
Guid! %

CustomerId& 0
,0 1
IEnumerable2 =
<= >"
CreateOrderItemRequest> T
>T U
ItemsV [
)[ \
;\ ]
public

 
record

 
CreateOrderCommand

  
(

  !
Guid

! %

CustomerId

& 0
,

0 1
IEnumerable

2 =
<

= >"
CreateOrderItemRequest

> T
>

T U
Items

V [
)

[ \
:

] ^
IRequest

_ g
<

g h
Order

h m
>

m n
;

n o
public 
record 
GetOrderByIdQuery 
(  
Guid  $
Id% '
)' (
:) *
IRequest+ 3
<3 4
Order4 9
?9 :
>: ;
;; <
public 
record 
GetOrdersQuery 
: 
IRequest '
<' (
IReadOnlyCollection( ;
<; <
Order< A
>A B
>B C
;C D
public 
record $
UpdateOrderStatusCommand &
(& '
Guid' +
Id, .
,. /
OrderStatus0 ;
Status< B
)B C
:D E
IRequestF N
<N O
OrderO T
>T U
;U V
public 
record $
UpdateOrderStatusRequest &
(& '
OrderStatus' 2
Status3 9
)9 :
;: ;
public 
	interface 
IOrderRepository !
{ 
Task 
< 	
Order	 
> 
AddAsync 
( 
Order 
order $
,$ %
CancellationToken& 7
cancellationToken8 I
=J K
defaultL S
)S T
;T U
Task 
< 	
Order	 
? 
> 
GetByIdAsync 
( 
Guid "
id# %
,% &
CancellationToken' 8
cancellationToken9 J
=K L
defaultM T
)T U
;U V
Task 
< 	
IReadOnlyCollection	 
< 
Order "
>" #
># $
GetAllAsync% 0
(0 1
CancellationToken1 B
cancellationTokenC T
=U V
defaultW ^
)^ _
;_ `
Task 
UpdateAsync	 
( 
Order 
order  
,  !
CancellationToken" 3
cancellationToken4 E
=F G
defaultH O
)O P
;P Q
} 
public 
	interface 
IOrderService 
{ 
Task 
< 	
Order	 
> 
CreateAsync 
( 
CreateOrderRequest .
request/ 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
Task 
< 	
Order	 
? 
> 
GetByIdAsync 
( 
Guid "
id# %
,% &
CancellationToken' 8
cancellationToken9 J
=K L
defaultM T
)T U
;U V
Task   
<   	
IReadOnlyCollection  	 
<   
Order   "
>  " #
>  # $
GetAllAsync  % 0
(  0 1
CancellationToken  1 B
cancellationToken  C T
=  U V
default  W ^
)  ^ _
;  _ `
Task!! 
<!! 	
Order!!	 
>!! 
UpdateStatusAsync!! !
(!!! "
Guid!!" &
id!!' )
,!!) *
OrderStatus!!+ 6
status!!7 =
,!!= >
CancellationToken!!? P
cancellationToken!!Q b
=!!c d
default!!e l
)!!l m
;!!m n
}"" 
public$$ 
sealed$$ 
class$$ %
CreateOrderCommandHandler$$ -
:$$. /
IRequestHandler$$0 ?
<$$? @
CreateOrderCommand$$@ R
,$$R S
Order$$T Y
>$$Y Z
{%% 
private&& 
readonly&& 
IOrderRepository&& %
_orderRepository&&& 6
;&&6 7
public(( 
%
CreateOrderCommandHandler(( $
((($ %
IOrderRepository((% 5
orderRepository((6 E
)((E F
{)) 
_orderRepository** 
=** 
orderRepository** *
;*** +
}++ 
public-- 

async-- 
Task-- 
<-- 
Order-- 
>-- 
Handle-- #
(--# $
CreateOrderCommand--$ 6
request--7 >
,--> ?
CancellationToken--@ Q
cancellationToken--R c
)--c d
{.. 
if// 

(// 
request// 
.// 

CustomerId// 
==// !
Guid//" &
.//& '
Empty//' ,
)//, -
throw00 
new00 
ArgumentException00 '
(00' (
$str00( A
,00A B
nameof00C I
(00I J
request00J Q
)00Q R
)00R S
;00S T
var22 
items22 
=22 
request22 
.22 
Items22 !
.22! "
ToList22" (
(22( )
)22) *
;22* +
if33 

(33 
items33 
.33 
Count33 
==33 
$num33 
)33 
throw44 
new44 
ArgumentException44 '
(44' (
$str44( R
,44R S
nameof44T Z
(44Z [
request44[ b
)44b c
)44c d
;44d e
var66 
order66 
=66 
new66 
Order66 
(66 
request66 %
.66% &

CustomerId66& 0
)660 1
;661 2
foreach88 
(88 
var88 
item88 
in88 
items88 "
)88" #
{99 	
order:: 
.:: 
AddItem:: 
(:: 
item:: 
.:: 
	ProductId:: (
,::( )
item::* .
.::. /
ProductName::/ :
,::: ;
item::< @
.::@ A
Quantity::A I
,::I J
item::K O
.::O P
	UnitPrice::P Y
)::Y Z
;::Z [
};; 	
return== 
await== 
_orderRepository== %
.==% &
AddAsync==& .
(==. /
order==/ 4
,==4 5
cancellationToken==6 G
)==G H
;==H I
}>> 
}?? 
publicAA 
sealedAA 
classAA $
GetOrderByIdQueryHandlerAA ,
:AA- .
IRequestHandlerAA/ >
<AA> ?
GetOrderByIdQueryAA? P
,AAP Q
OrderAAR W
?AAW X
>AAX Y
{BB 
privateCC 
readonlyCC 
IOrderRepositoryCC %
_orderRepositoryCC& 6
;CC6 7
publicEE 
$
GetOrderByIdQueryHandlerEE #
(EE# $
IOrderRepositoryEE$ 4
orderRepositoryEE5 D
)EED E
{FF 
_orderRepositoryGG 
=GG 
orderRepositoryGG *
;GG* +
}HH 
publicJJ 

TaskJJ 
<JJ 
OrderJJ 
?JJ 
>JJ 
HandleJJ 
(JJ 
GetOrderByIdQueryJJ 0
requestJJ1 8
,JJ8 9
CancellationTokenJJ: K
cancellationTokenJJL ]
)JJ] ^
=>KK 

_orderRepositoryKK 
.KK 
GetByIdAsyncKK (
(KK( )
requestKK) 0
.KK0 1
IdKK1 3
,KK3 4
cancellationTokenKK5 F
)KKF G
;KKG H
}LL 
publicNN 
sealedNN 
classNN !
GetOrdersQueryHandlerNN )
:NN* +
IRequestHandlerNN, ;
<NN; <
GetOrdersQueryNN< J
,NNJ K
IReadOnlyCollectionNNL _
<NN_ `
OrderNN` e
>NNe f
>NNf g
{OO 
privatePP 
readonlyPP 
IOrderRepositoryPP %
_orderRepositoryPP& 6
;PP6 7
publicRR 
!
GetOrdersQueryHandlerRR  
(RR  !
IOrderRepositoryRR! 1
orderRepositoryRR2 A
)RRA B
{SS 
_orderRepositoryTT 
=TT 
orderRepositoryTT *
;TT* +
}UU 
publicWW 

TaskWW 
<WW 
IReadOnlyCollectionWW #
<WW# $
OrderWW$ )
>WW) *
>WW* +
HandleWW, 2
(WW2 3
GetOrdersQueryWW3 A
requestWWB I
,WWI J
CancellationTokenWWK \
cancellationTokenWW] n
)WWn o
=>XX 

_orderRepositoryXX 
.XX 
GetAllAsyncXX '
(XX' (
cancellationTokenXX( 9
)XX9 :
;XX: ;
}YY 
public[[ 
sealed[[ 
class[[ +
UpdateOrderStatusCommandHandler[[ 3
:[[4 5
IRequestHandler[[6 E
<[[E F$
UpdateOrderStatusCommand[[F ^
,[[^ _
Order[[` e
>[[e f
{\\ 
private]] 
readonly]] 
IOrderRepository]] %
_orderRepository]]& 6
;]]6 7
public__ 
+
UpdateOrderStatusCommandHandler__ *
(__* +
IOrderRepository__+ ;
orderRepository__< K
)__K L
{`` 
_orderRepositoryaa 
=aa 
orderRepositoryaa *
;aa* +
}bb 
publicdd 

asyncdd 
Taskdd 
<dd 
Orderdd 
>dd 
Handledd #
(dd# $$
UpdateOrderStatusCommanddd$ <
requestdd= D
,ddD E
CancellationTokenddF W
cancellationTokenddX i
)ddi j
{ee 
varff 
orderff 
=ff 
awaitff 
_orderRepositoryff *
.ff* +
GetByIdAsyncff+ 7
(ff7 8
requestff8 ?
.ff? @
Idff@ B
,ffB C
cancellationTokenffD U
)ffU V
??gg 
throwgg 
newgg  
KeyNotFoundExceptiongg -
(gg- .
$"gg. 0
$strgg0 6
{gg6 7
requestgg7 >
.gg> ?
Idgg? A
}ggA B
$strggB Q
"ggQ R
)ggR S
;ggS T
orderii 
.ii 
UpdateStatusii 
(ii 
requestii "
.ii" #
Statusii# )
)ii) *
;ii* +
awaitjj 
_orderRepositoryjj 
.jj 
UpdateAsyncjj *
(jj* +
orderjj+ 0
,jj0 1
cancellationTokenjj2 C
)jjC D
;jjD E
returnll 
orderll 
;ll 
}mm 
}nn 
publicpp 
sealedpp 
classpp 
OrderServicepp  
:pp! "
IOrderServicepp# 0
{qq 
privaterr 
readonlyrr 
IOrderRepositoryrr %
_orderRepositoryrr& 6
;rr6 7
publictt 

OrderServicett 
(tt 
IOrderRepositorytt (
orderRepositorytt) 8
)tt8 9
{uu 
_orderRepositoryvv 
=vv 
orderRepositoryvv *
;vv* +
}ww 
publicyy 

Taskyy 
<yy 
Orderyy 
>yy 
CreateAsyncyy "
(yy" #
CreateOrderRequestyy# 5
requestyy6 =
,yy= >
CancellationTokenyy? P
cancellationTokenyyQ b
=yyc d
defaultyye l
)yyl m
=>zz 

newzz %
CreateOrderCommandHandlerzz (
(zz( )
_orderRepositoryzz) 9
)zz9 :
.zz: ;
Handlezz; A
(zzA B
newzzB E
CreateOrderCommandzzF X
(zzX Y
requestzzY `
.zz` a

CustomerIdzza k
,zzk l
requestzzm t
.zzt u
Itemszzu z
)zzz {
,zz{ |
cancellationToken	zz} é
)
zzé è
;
zzè ê
public|| 

Task|| 
<|| 
Order|| 
?|| 
>|| 
GetByIdAsync|| $
(||$ %
Guid||% )
id||* ,
,||, -
CancellationToken||. ?
cancellationToken||@ Q
=||R S
default||T [
)||[ \
=>}} 

new}} $
GetOrderByIdQueryHandler}} '
(}}' (
_orderRepository}}( 8
)}}8 9
.}}9 :
Handle}}: @
(}}@ A
new}}A D
GetOrderByIdQuery}}E V
(}}V W
id}}W Y
)}}Y Z
,}}Z [
cancellationToken}}\ m
)}}m n
;}}n o
public 

Task 
< 
IReadOnlyCollection #
<# $
Order$ )
>) *
>* +
GetAllAsync, 7
(7 8
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
=>
ÄÄ 

new
ÄÄ #
GetOrdersQueryHandler
ÄÄ $
(
ÄÄ$ %
_orderRepository
ÄÄ% 5
)
ÄÄ5 6
.
ÄÄ6 7
Handle
ÄÄ7 =
(
ÄÄ= >
new
ÄÄ> A
GetOrdersQuery
ÄÄB P
(
ÄÄP Q
)
ÄÄQ R
,
ÄÄR S
cancellationToken
ÄÄT e
)
ÄÄe f
;
ÄÄf g
public
ÇÇ 

Task
ÇÇ 
<
ÇÇ 
Order
ÇÇ 
>
ÇÇ 
UpdateStatusAsync
ÇÇ (
(
ÇÇ( )
Guid
ÇÇ) -
id
ÇÇ. 0
,
ÇÇ0 1
OrderStatus
ÇÇ2 =
status
ÇÇ> D
,
ÇÇD E
CancellationToken
ÇÇF W
cancellationToken
ÇÇX i
=
ÇÇj k
default
ÇÇl s
)
ÇÇs t
=>
ÉÉ 

new
ÉÉ -
UpdateOrderStatusCommandHandler
ÉÉ .
(
ÉÉ. /
_orderRepository
ÉÉ/ ?
)
ÉÉ? @
.
ÉÉ@ A
Handle
ÉÉA G
(
ÉÉG H
new
ÉÉH K&
UpdateOrderStatusCommand
ÉÉL d
(
ÉÉd e
id
ÉÉe g
,
ÉÉg h
status
ÉÉi o
)
ÉÉo p
,
ÉÉp q 
cancellationTokenÉÉr É
)ÉÉÉ Ñ
;ÉÉÑ Ö
}ÑÑ ë
BC:\projects\orderengine\OrderEngine.Application\LoggingBehavior.cs
	namespace 	
OrderEngine
 
. 
Application !
;! "
public 
sealed 
class 
LoggingBehavior #
<# $
TRequest$ ,
,, -
	TResponse. 7
>7 8
:9 :
IPipelineBehavior; L
<L M
TRequestM U
,U V
	TResponseW `
>` a
where 	
TRequest
 
: 
notnull 
{		 
private

 
readonly

 
ILogger

 
<

 
LoggingBehavior

 ,
<

, -
TRequest

- 5
,

5 6
	TResponse

7 @
>

@ A
>

A B
_logger

C J
;

J K
public 

LoggingBehavior 
( 
ILogger "
<" #
LoggingBehavior# 2
<2 3
TRequest3 ;
,; <
	TResponse= F
>F G
>G H
loggerI O
)O P
{ 
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
	TResponse 
>  
Handle! '
(' (
TRequest( 0
request1 8
,8 9"
RequestHandlerDelegate: P
<P Q
	TResponseQ Z
>Z [
next\ `
,` a
CancellationTokenb s
cancellationToken	t Ö
)
Ö Ü
{ 
var 
requestName 
= 
typeof  
(  !
TRequest! )
)) *
.* +
Name+ /
;/ 0
var 
	stopwatch 
= 
	Stopwatch !
.! "
StartNew" *
(* +
)+ ,
;, -
_logger 
. 
LogInformation 
( 
$str O
,O P
requestNameQ \
,\ ]
request^ e
)e f
;f g
try 
{ 	
var 
response 
= 
await  
next! %
(% &
)& '
;' (
	stopwatch 
. 
Stop 
( 
) 
; 
_logger 
. 
LogInformation "
(" #
$str \
,\ ]
requestName 
, 
	stopwatch   
.   
ElapsedMilliseconds   -
,  - .
response!! 
)!! 
;!! 
return## 
response## 
;## 
}$$ 	
catch%% 
(%% 
	Exception%% 
ex%% 
)%% 
{&& 	
	stopwatch'' 
.'' 
Stop'' 
('' 
)'' 
;'' 
_logger(( 
.(( 
LogError(( 
((( 
ex(( 
,((  
$str((! c
,((c d
requestName((e p
,((p q
	stopwatch((r {
.(({ | 
ElapsedMilliseconds	((| è
)
((è ê
;
((ê ë
throw)) 
;)) 
}** 	
}++ 
},, 