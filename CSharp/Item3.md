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

**형변환 방법**
1. as 연산자
2. 컴파일러의 캐스트 연산자

형변환의 경우 캐스팅 보단 as 연산자를 사용하는것이 좋음  

**as 연산자의 장점**
1. 더 안전함
2. 런타임에 더 효율적

**as 연산자의 단점**
1. [사용자 정의 형변환](https://learn.microsoft.com/ko-kr/dotnet/csharp/language-reference/operators/user-defined-conversion-operators)은 수행되지 않음

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

캐시팅을 이용한 형변환 예시
```c#
//캐시팅을 이용한 형변환 예시
object o = Factory.getObject();

try
{
    MyType t;
    t = (MyType)o;
    //MyType타입의 t 객체 사용
}
catch(InvalidCastException e)
{
    // 오류 보고
}
```




