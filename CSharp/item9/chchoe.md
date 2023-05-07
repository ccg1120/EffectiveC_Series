# Effective c# 

# 아이템 9: 박싱과 언박싱을 최소화 하라!

값타입은 주로 값을 저장할 때 쓰는 저장소
- 다형적이지 못함

.NET Framework는 모든 타입의 **최상위 타입을 참조 타입인 System.Object**로 정의함 (Managed Language)


### **박싱이란?**

값 타입의 객체를 타입이 정해져 있지 않은 임의의 참조 타입 내부에 포함 시키는 방법

### **언박싱이란?**

박싱된 있는 참조 타입의 객체로 부터 **값 타입 객체의 복사본**을 가져오는 방법


박싱과 언박싱은 System.Object 타입이나 인터페이스 타입이 필요한 곳에서 값 타입을 사용하기 위해 반드시 필요한 매커니즘

**박싱, 언박싱 단점**
- 성능에 좋지 않은 영향을 미침
- 박싱, 언박싱 과정에서 임시 객체가 생성되기도 하는데 이때 예상치 못한 버그가 발생할 수 있음

**필요한 매커니즘이지만 박싱과 언박싱은 최대한 피하는 것이 좋음**

-------------------

### **박싱의 특성**

- 박싱을 하는 과정에서 새롭게 생성된 참조 타입의 객체는 힙에 생성됨

- 값 타입의 복사본이 새롭게 생성된 객체 내부에 저장됨

-------------------
### **박싱을 피하는 방법?**
.NET 2.0에 추가된 제네릭 클래스와 제네릭 메서드를 사용하면 박싱과 언박싱을 피할 수 있음  

하지만 .NET Framework에는 여전히 System.Object 타입의 객체를 요구하는 경우가 있음 (이런 API들은 여전히 박싱과 언박싱을 수행함)

### **박싱,언박싱의 위험성**
- 박싱과 언박싱은 **자동**으로 이뤄짐 (참조 타입을 요구하는 곳에 값 타입의 객체를 사용하면 컴파일러는 자동으로 코드를 생성함)

- 컴파일러는 **어떠한 경고 메시지 없이** 박싱 작업을 자연 스럽게 수행함

박싱 수행의 예

``` c#
int firstNumber;
int secondNumber;
int thirdNumber;

Console.WriteLine($"A few numbers: {firstNumber},{secondNumber}, {thirdNumber} "); 
// 보간 문자열은 System.Object객체에 대한 배열을 사용
// 참조 타입에 값 타입을 사용함 -> 박싱!
// 값타입을 문자열로 변환하기 위한 함수를 호출 하려면 반드시 박싱을 수행
```
위 코드는 다음과 아래과 같은 코드와 유사함
``` c#
int i = 25;
object o = (object)i; // 박싱
Console.WriteLine(o.ToString());
```

메서드 내에서 인자로 주어진 객체를 이용하여 문자열을 생성 하는 코드는 다음과 유사함

```c#
object firstParm = 5;
object o = firstParm;
int i = (int)o; // 언박싱

string output = i.ToString();
```

**성능상의 취약점을 개선 하고 싶다면 값타입의 객체를 직접 전달 하지 말고 문자열 인스턴스를 전달하는 것이 좋음**


``` c#
int firstNumber;
int secondNumber;
int thirdNumber;

Console.WriteLine($"A few numbers: {firstNumber.ToString()},{secondNumber.ToString()}, {thirdNumber.ToString()} "); 
//박싱과 언박싱을 피하기 위해 문자열 인스턴스를 전달함
```
----------------------


### **박싱을 피하는 규칙**
1. System.Object 타입으로 박싱이 이루어지는지 유심히 살피고 개선 할 것
2. 제네릭 컬렉션을 사용 할 것

**컬렌션 사용시 주의 사항**  
컬렉션으로 부터 객체를 가져오는 경우에도 박싱된 객체의 복사본을 가져옴 -> 매번 객체에 대한 복사가 일어남

이로 인한 버그가 발생하는 경우가 있음

값을 수정하는 간단한 예
```c#
//컬렉션 내에서 Person 타입을 사용
var attendees = new List<Person>(); //Person은 값 타입
Pserson p = new Person{Name = "Old Name"};
attendees.Add(p);


//Name 변경 시도
Pserson p2 = attendees[0];
p2.Name = "New Name";

//"Old Name"을 출력함 의도된게 아님 !에러!에러!
Console.WriteLine(attendees[0].ToString());

int i = 25;
object o = i; // 박싱
Console.WriteLine(o.ToString());
```

위 코드를 보게 되면 컴파일러는 `List<Person>`과 같이 닫힌 제네릭 타입을 생성

Person 객체를 저장할 때 박싱이 일어나지 않도록 했음

컬렉션에서 Person객체를 가져오는 순간 새로운 복사본이 생성됨

이후 속성 변경코드는 복사본을 대상으로 이뤄짐

attendees[0].ToString() 여기서도 다른 복사본이 생성됨

**여러 이유에서라도 변경 불가능한 값 타입(immutable)을 만드는것이 여러모로 좋음**

결론적으로 값 타입은 System.Object타입이나 인터페이스 타입으로 변경할 수 있음

변환 작업은 암시적이며 어떤 부분에서 변환 작업이 이뤄졌는지 찾기도 어려움

이로 인해 버그가 발생 할 수 있으며 값 타입을 다형적으러 처리하는 과정에서 성능을 느리게 할 수 있음을 인지 해야함


[Boxing 및 Unboxing](https://learn.microsoft.com/ko-kr/dotnet/csharp/programming-guide/types/boxing-and-unboxing)

[Boxing 및 Unboxing 성능](https://learn.microsoft.com/ko-kr/dotnet/framework/performance/performance-tips)

[값 타입에 ref 는?](https://learn.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/ref)




