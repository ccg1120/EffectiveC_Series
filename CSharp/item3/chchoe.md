# Effective c# 

# 아이템 3:캐스트보다는 is, as가 좋다.

c#은 **정적 타이핑을 수행하는 언어**

정적 타입 언어(Static Typing Language)  
- 컴파일 시간에 모든 변수의 데이터 타입을 결정하고 확인
- 코드에 대한 오류를 미리 검출할 수 있도록 도와줌
- 대표 언어 (C, C++, Java, C#)

동적 타입 언어(Dynamic Typing Language)  
- 컴파일 시간에 변수의 데이터 타입을 결정하지 않음
- 실행 시간에 변수의 데이터 타입을 결정하고 처리
- 대표 언어 (Python, Ruby, JavaScrip)


정적 타입 언어지만 런타임에서 타입을 확인해야 하는 경우가 발생함

----------------------------

**형변환 방법**
1. as 연산자
2. 컴파일러의 캐스트 연산자

형변환의 경우 캐스팅 보단 as 연산자를 사용하는것이 좋음  

**as 연산자의 장점**
1. 더 안전함 (컴파일 단에서 확인 가능하기 때문에?)
2. 런타임에 더 효율적 (불필요 코드가 없음)

**as 연산자의 단점**
1. [사용자 정의 형변환](https://learn.microsoft.com/ko-kr/dotnet/csharp/language-reference/operators/user-defined-conversion-operators)은 수행되지 않음

----------------------------

as 연산자를 이용한 형변환 예시
```c#
//as 연산자를 이용한 형변환 예시
object o = Factory.getObject();

MyType t = o as Mytype;

if(t != null)
{
    //MyType타입의 t 객체 사용
}
else
{
    //오류 보고
}
```

캐스팅을 이용한 형변환 예시
```c#
//캐스팅을 이용한 형변환 예시
object o = Factory.getObject();

try
{
    MyType t;
    t = (MyType)o;

    if( t == null) 
    {
        // null은 어떤 형이든 변환이 가능 하기때문에 null확인 필요
        //오류 보고
    }
    //MyType타입의 t 객체 사용    
}
catch(InvalidCastException e)
{
    // 오류 보고
}
```

as 연산자를 이용할 경우 ``` try/catch ``` 문이 없어 성능이 좋음

캐스팅을 이용할 경우 null 확인이 필요 없을거 같지만 null은 어떤 참조 타입으로도 형변환이 가능 하기 때문에 null 확인이 필요함

as 연산자는 형변환을 수행 할 수 없거나 null을 대상으로 형변환이 수행 되면 null을 반환 하게 됨

----------------------------

**as 연산자와 캐스팅의 가장 큰 차이는?**
- 사용자 정의 형변환을 어떻게 다루는가 

어떤 차이가 있나?

as, is연산자는 런타임에 객체의 타입을 확인, 필요에 따라 박싱을 수행하는 것외에는 아무것도 하지 않음
(상속 또는 지정한 타입이여야함)

캐스팅의 경우 지정한 타입으로 변환 하기 위해 형변환 연산자가 개입 될 수 있음

대표적 형변환 연산자 예시 -> 숫자 타입에 대한 형변환 (long 타입을 short 으로 캐스팅 하는 경우 정보 손실)



```c#
public Class SecondType
{
    private Mytype _value;

    //사용자 형변환 연산자 
    // SecondType을 MyType으로 변환
    public static implicit operator MyType(SecondType t)
    {
        return t._value;
    }
}

// 1번째 버전
object o = Factory.getObject();
// o는 SecondType 타입

MyType t = o as MyType;

if (t != null)
{
    //MyType타입의 t 객체 사용
}
else
{
    //오류 보고
}

// 2번째 버전
object o = Factory.getObject();

try
{
    MyType t;
    t = (MyType)o;

    if( t == null) 
    {
        // null은 어떤 형이든 변환이 가능 하기때문에 null확인 필요
        //오류 보고
    }
    //MyType타입의 t 객체 사용    
}
catch(InvalidCastException e)
{
    // 오류 보고
}
```

사용자 형변환이 있으니 2버전은 성공 할거 같지만 둘다 형변환에 실패함

왜일까?

컴파일러는 런타임에 객체가 어떤 타입인지 예측하지 못함 왜냐면 컴파일러는 어떤 타입으로 선언 됐는지만 추적 하기 때문에 
2번째 버전도 실패함 

(object 타입을 MyType으로 형변환 되는지만 확인한다. 하지만 런타임에 o는 SecondType임으로 형변환에 실패)


```c#
// 2번째 버전의 어거지 버전
object o = Factory.getObject();

SecondType st = o as SecondType; //SecondType 타입으로 선언됨
// 이런 코드는 작성되선 안됨 그냥 예시일뿐
try
{
    MyType t;
    t = (MyType)st; //SecondType은 MyType으로 사용자 형변환 정의 되어 있음으로 형변환 가능

    if( t == null) 
    {
        // null은 어떤 형이든 변환이 가능 하기때문에 null확인 필요
        //오류 보고
    }
    //MyType타입의 t 객체 사용    
}
catch(InvalidCastException e)
{
    // 오류 보고
}
```

사용자 정의 형변환 연산자는 객체의 **런타임 타입이 아닌 컴파일 타임 타입**에 맞춰 수행된다는 점에 다시 한번 유의 할 것  


```c#
t = (MyType) st; //캐스팅 방식을 사용하면 st의 타입에 따라 결과가 바뀌게됨(st의 타입에 따라 사용자 형변환 따라 결과가 틀림`)

t =  st as MyType; // as 연산자 방식 일 경우 st가 Mytype 이거나 MyType을 상속한 타입이 아니면 컴파일에러가 발생
```

as 연산자를 사용 할 수 없는 경우에 대하여 알아보자

```c#
object o = Factory.getObject();
int i = o as int; // int는 값 타입이고 null이 될 수 없기 때문에 as를 사용 할 수 없음
```

저런 경우 캐스팅을 써서 해결할까 하지만 as 연산자를 사용하면서 해결 할 수 있음

```c#
object o = Factory.getObject();
var i = o as int ?; // nullable 타입으로 형변환 후 null인지 확인

if(i != null)
{
    //int 사용
}
```
int? 및 int는 각각 System.Nullable<System.Int32> 및 System.Int32로 표현
[nullable 참조 형식](https://learn.microsoft.com/ko-kr/dotnet/csharp/nullable-references)

----------------------------
foreach 루프를 사용 할 경우 IEnumerable 인터페이스를 사용함  
이를 구현 하는 과정에서 형변환을 수행하게 됨

``` c#
public void UseCollection(IEnumerable theCollection )
{
    foreach(MyType t in theCollection)
        t.DoStuff();
}

//위 코드는 아래 코드와 유사하게 생성됨
public void UseCollection(IEnumerable theCollection )
{
    IEnumerator it = theCollection.GetEnumerator();
    while (it.MoveNext())
    {
        MyType t = (MyType)it.Current;
        t.DoStuff();
    }
}
```

foreach문은 형변환을 지원해야 하는 캐스팅을 사용하면 대상 타입을 구분 할 필요가 없어짐

이로 인해 InvalidCastException이 발상 할 가능성이 있음

컬렉션 내부의 객체가 런타임에 어떤 타입인지 형변환 가능한지 등을 확인 하지 않음  
System.Object 타입으로 변환 가능한지 와 다시 루프 변수 타입으로 형변환 가능한지만 확인 할뿐임


**객체의 정확한 타입을 알고 싶다면??**

GetType()함수를 사용하면 정확한 타입을 가져옴  
(is연산자는 다형성 규칙을 준수 하기때문에 정확하지 않을 수 있음)

----------------------------

C# 4.0 qnxjsms dynamic, 런타임 타입 확인 기능이 추가됨에 따라 타입 시스템이 더욱 풍성하게 변경 되었음

사용자의 의도를 명확히 표련할 수 있는 is와 as 연산자를 사용 하라. 형변환을 위해 다양한 방법을 사용 할 수있지만  
서로 다르게 동작 한다는점을 유념해야함. is와 as 연산자는 거의 항상 예상대로 동작하며 대상 객체를 올바르게 형변환 할 수 있는 경우에만 성공함.  

is와 as를 사용하는 것이 의도하지 않는 부작용이나 예상치 못한 문제를 피할 수 있는 좋은 방법임












 






