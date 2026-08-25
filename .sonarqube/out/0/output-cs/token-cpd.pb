ÓP
<C:\projects\orderengine\OrderEngine.Domain\OrderAggregate.cs
	namespace 	
OrderEngine
 
. 
Domain 
; 
public 
enum 
OrderStatus 
{ 
Pending 
, 
	Confirmed 
, 
	Cancelled 
} 
public

 
sealed

 
class

 
	OrderItem

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
=) *
Guid+ /
./ 0
NewGuid0 7
(7 8
)8 9
;9 :
public 

string 
	ProductId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

string 
ProductName 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

int 
Quantity 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

decimal 
	UnitPrice 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

decimal 
Total 
=> 
Quantity $
*% &
	UnitPrice' 0
;0 1
public 

	OrderItem 
( 
string 
	productId %
,% &
string' -
productName. 9
,9 :
int; >
quantity? G
,G H
decimalI P
	unitPriceQ Z
)Z [
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
	productId& /
)/ 0
)0 1
throw 
new 
ArgumentException '
(' (
$str( @
,@ A
nameofB H
(H I
	productIdI R
)R S
)S T
;T U
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
productName& 1
)1 2
)2 3
throw 
new 
ArgumentException '
(' (
$str( B
,B C
nameofD J
(J K
productNameK V
)V W
)W X
;X Y
if 

( 
quantity 
<= 
$num 
) 
throw 
new '
ArgumentOutOfRangeException 1
(1 2
nameof2 8
(8 9
quantity9 A
)A B
,B C
$strD i
)i j
;j k
if 

( 
	unitPrice 
<= 
$num 
) 
throw   
new   '
ArgumentOutOfRangeException   1
(  1 2
nameof  2 8
(  8 9
	unitPrice  9 B
)  B C
,  C D
$str  E e
)  e f
;  f g
	ProductId"" 
="" 
	productId"" 
;"" 
ProductName## 
=## 
productName## !
;##! "
Quantity$$ 
=$$ 
quantity$$ 
;$$ 
	UnitPrice%% 
=%% 
	unitPrice%% 
;%% 
}&& 
}'' 
public)) 
sealed)) 
class)) 
Order)) 
{** 
private++ 
readonly++ 
List++ 
<++ 
	OrderItem++ #
>++# $
_items++% +
=++, -
new++. 1
(++1 2
)++2 3
;++3 4
public-- 

Guid-- 
Id-- 
{-- 
get-- 
;-- 
private-- !
set--" %
;--% &
}--' (
=--) *
Guid--+ /
.--/ 0
NewGuid--0 7
(--7 8
)--8 9
;--9 :
public.. 

Guid.. 

CustomerId.. 
{.. 
get..  
;..  !
private.." )
set..* -
;..- .
}../ 0
public// 

DateTime// 
	CreatedAt// 
{// 
get//  #
;//# $
private//% ,
set//- 0
;//0 1
}//2 3
=//4 5
DateTime//6 >
.//> ?
UtcNow//? E
;//E F
public00 

OrderStatus00 
Status00 
{00 
get00  #
;00# $
private00% ,
set00- 0
;000 1
}002 3
=004 5
OrderStatus006 A
.00A B
Pending00B I
;00I J
public11 

ICollection11 
<11 
	OrderItem11  
>11  !
Items11" '
=>11( *
_items11+ 1
;111 2
public22 

decimal22 
Total22 
=>22 
_items22 "
.22" #
Sum22# &
(22& '
item22' +
=>22, .
item22/ 3
.223 4
Total224 9
)229 :
;22: ;
public44 

Order44 
(44 
Guid44 

customerId44  
)44  !
{55 
if66 

(66 

customerId66 
==66 
Guid66 
.66 
Empty66 $
)66$ %
throw77 
new77 
ArgumentException77 '
(77' (
$str77( A
,77A B
nameof77C I
(77I J

customerId77J T
)77T U
)77U V
;77V W

CustomerId99 
=99 

customerId99 
;99  
}:: 
public<< 

void<< 
AddItem<< 
(<< 
string<< 
	productId<< (
,<<( )
string<<* 0
productName<<1 <
,<<< =
int<<> A
quantity<<B J
,<<J K
decimal<<L S
	unitPrice<<T ]
)<<] ^
{== 
_items>> 
.>> 
Add>> 
(>> 
new>> 
	OrderItem>>  
(>>  !
	productId>>! *
,>>* +
productName>>, 7
,>>7 8
quantity>>9 A
,>>A B
	unitPrice>>C L
)>>L M
)>>M N
;>>N O
}?? 
publicAA 

voidAA 

RemoveItemAA 
(AA 
GuidAA 
itemIdAA  &
)AA& '
{BB 
varCC 
itemCC 
=CC 
_itemsCC 
.CC 
FirstOrDefaultCC (
(CC( )
xCC) *
=>CC+ -
xCC. /
.CC/ 0
IdCC0 2
==CC3 5
itemIdCC6 <
)CC< =
;CC= >
ifDD 

(DD 
itemDD 
isDD 
nullDD 
)DD 
throwEE 
newEE %
InvalidOperationExceptionEE /
(EE/ 0
$strEE0 N
)EEN O
;EEO P
_itemsGG 
.GG 
RemoveGG 
(GG 
itemGG 
)GG 
;GG 
}HH 
publicJJ 

voidJJ 
ConfirmJJ 
(JJ 
)JJ 
{KK 
ifLL 

(LL 
StatusLL 
==LL 
OrderStatusLL !
.LL! "
	CancelledLL" +
)LL+ ,
throwMM 
newMM %
InvalidOperationExceptionMM /
(MM/ 0
$strMM0 b
)MMb c
;MMc d
ifOO 

(OO 
StatusOO 
!=OO 
OrderStatusOO !
.OO! "
PendingOO" )
)OO) *
throwPP 
newPP %
InvalidOperationExceptionPP /
(PP/ 0
$strPP0 W
)PPW X
;PPX Y
StatusRR 
=RR 
OrderStatusRR 
.RR 
	ConfirmedRR &
;RR& '
}SS 
publicUU 

voidUU 
CancelUU 
(UU 
)UU 
{VV 
ifWW 

(WW 
StatusWW 
==WW 
OrderStatusWW !
.WW! "
	ConfirmedWW" +
)WW+ ,
throwXX 
newXX %
InvalidOperationExceptionXX /
(XX/ 0
$strXX0 a
)XXa b
;XXb c
ifZZ 

(ZZ 
StatusZZ 
==ZZ 
OrderStatusZZ !
.ZZ! "
	CancelledZZ" +
)ZZ+ ,
throw[[ 
new[[ %
InvalidOperationException[[ /
([[/ 0
$str[[0 R
)[[R S
;[[S T
Status]] 
=]] 
OrderStatus]] 
.]] 
	Cancelled]] &
;]]& '
}^^ 
public`` 

void`` 
UpdateStatus`` 
(`` 
OrderStatus`` (
status``) /
)``/ 0
{aa 
switchbb 
(bb 
statusbb 
)bb 
{cc 	
casedd 
OrderStatusdd 
.dd 
Pendingdd $
:dd$ %
ifee 
(ee 
Statusee 
!=ee 
OrderStatusee )
.ee) *
Pendingee* 1
)ee1 2
throwff 
newff %
InvalidOperationExceptionff 7
(ff7 8
$strff8 i
)ffi j
;ffj k
returngg 
;gg 
casehh 
OrderStatushh 
.hh 
	Confirmedhh &
:hh& '
Confirmii 
(ii 
)ii 
;ii 
returnjj 
;jj 
casekk 
OrderStatuskk 
.kk 
	Cancelledkk &
:kk& '
Cancelll 
(ll 
)ll 
;ll 
returnmm 
;mm 
defaultnn 
:nn 
throwoo 
newoo '
ArgumentOutOfRangeExceptionoo 5
(oo5 6
nameofoo6 <
(oo< =
statusoo= C
)ooC D
,ooD E
statusooF L
,ooL M
$strooN i
)ooi j
;ooj k
}pp 	
}qq 
}rr 