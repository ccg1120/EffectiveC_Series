# Effective c# 

# 아이템 25: 타입 매개변수로 인스턴스 필드를 만들 필요가 없다면 제네릭 메서드를 정의하라

유틸리티 성격의 클래스를 만드는 경우에는 일반 클래스 내에 제네릭 메서드를 작성하는 편이 훨씬 좋음.

왜?

제네릭 클래스를 작성하면 컴파일러 입장에서 타입 매개변수에 대한 제약조건을 고려하여 컴파일 해야 하기 때문

제네릭 메서드를 정의하면 매개변수에 대한 제약 조건을 메서드 수준으로 지정할 수 있음

제네릭 클래스를 정의 하면 클래스 전체에 대하여 제약 조건을 고려해야함
- 제약 조건의 작용 범위가 넓어지면 코드를 수정하기 점점 더 까다로워짐

```c#
public static class Utilis<T>
{
    public static T Max(T left, T right) =>
        Comparer<T>.Default.Compare(left, right) < 0 ? right : left;

    public static T Min(T left, T right) =>
        Comparer<T>.Default.Compare(left, right) < 0 ? left :right;
}

double d1 = 4;
double d2 = 5;
double max = Utils<double>.Max(d1, d2);
//이코드는 완벽해 보임

string foo = "foo";
string bar = "bar";
string sMax = Utils<string>.Max(foo, bar);

```
사용성도 떨어지며 문제가 있는 코드임 왜?

1. 메서드 호출 시 마다 매번 타입 매개변수를 명시적으로 지정해야함
2. .NET Framework에 포함된 상당히 많은 타입이 이미 Max, Min메서드를 가지고 있음

동작에는 문제가 없지만 매번 타입 매개변수가 IComparer<T>를 구현하는지 런타임에 확인 후 메서드를 온전히 호출 함 (비효율적임)

개발자 라면 타입에 가장 잘 부합하는 최상의 메서드가 선택되길를 원함.

클래스 내에 제네릭 메서드를 구현하면 기능을 구현하기 좀더 용이함

```c#
public static class Utils //일반 클래스
{
    //제네릭 메서드
    public static T Max<T>(T left, T right) =>
        Comparer<T>.Default.Compare(left, right) < 0 ? right : left;
    //정의된 Max를 사용하여 메서드를 구현
    public static double Max<T>(double left, double right) => 
        Math.Max(left, right);

    public static T Min<T>(T left, T right) =>
        Comparer<T>.Default.Compare(left, right) < 0 ? left : right ;
    
    public static double Min<T>(double left, double right) => 
        Math.Min(left, right);
}
```

위 예제를 보면 Utils 클래스는 제네릭 클래스가 아님, Min, Max에 대하여 여러 개의 오버라이딩 메서드를 작성함

타입을 구체적으로 지정한 메서드는 제네릭 버전 보다 효율적으로 동작함 (**아이템 3: 캐스트보다는 is,as가 좋다**)


```c#
doudle d1 = 4;
doudle d2 = 5;
doudle max = Utils.Max(d1, d2); // 타입 지정된 메서드를 사용함으로써 효율적임

string foo = "foo";
string bar = "bar";
string sMax = Utils.Max(foo, bar); // 타입 매개변수를 일일이 써주지 않아도 되어 사용성이 편리해짐

doudle? d3 = 4;
doudle? d4 = 5;
doudle? max2 = Utils.Max(d1, d2).Value;
```

매개변수의 타입이 정확히 일치하는 메서드가 이미 구현되어 있다면 그 메서드가 호출되고 타입이 일치하는 메서드가 없다면 제네릭 메서드가 호출됨

```c#
public class CommaSeparatedListBuilder
{
    private StringBuilder storage = new StringBuilder();
    
    public void Add<T>(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            if(storage.Lenth > 0 )
            {
                storage.Append(", ");
                storage.Append("\"");
                storage.Append(item.ToString());
                storage.Append("\"");
            }

        }
    }

    public override string ToString() => storage.ToString();
}
```

위 메서드는 여러 가지 타입을 지원함. 사용한 적이 없는 타입을 사용하면 컴파일러가 알아서 타입에 맞춰 Add<T>를 추가로 특화함.

동일한 기능을 제니릭 클래스로 작성 하면 단 하나의 타입에 대해서만 동작함

대충 아래 같은 느낌?
```c#
public class CommaSeparatedListBuilder<T>
{
    private StringBuilder storage = new StringBuilder();
    public void Add(IEnumerable<T> items) // 제네릭 클래스 임으로 T가 고정이 되어버림
    {
        foreach (T item in items)
        {
            if(storage.Lenth > 0 )
            {
                storage.Append(", ");
                storage.Append("\"");
                storage.Append(item.ToString());
                storage.Append("\"");
            }

        }
    }
}
```

일반 클래스 내에 제네릭 메서드를 만들어 타입별로 특화되도록 코드를 작성 하는것이 좋은 장점일 때가 있음  
동일한 메서드에 대해서 다른 타입을 사용하더라도 제네릭 클래스 처럼 추가적으로 객체를 생성 할 필요가 없음
```c#
// 위 예제대로 하면 각자 객체를 만들어야함
CommaSeparatedListBuilder<string> stringBuilder;
CommaSeparatedListBuilder<int> intBuilder;
```

제네릭 메서드를 사용하는 것이 무조건 장점만 있는 것은 아님.

두가지 경우는 반드시 제네릭 클래스를 만들어야함

1. 타입 매개변수로 주어진 타입으로 내부 상태를 유지해야 하는 경우(컬렉션이 바로 이런 경우)
2. 제네릭 인터페이스를 구현하는 클래스를 만들어야 할 경우

이 경우를 제외하면 제네릭 메서드를 구현하여 사용하는 것이 좋음

API를 작성할 때는 가능한 이런 방식의 구현이 좋음(명시적 타입 지정 하지 않게 하는 방식)

특정 타입에 대해 더욱 효율적으로 동작하는 특화된 메서드를 추가하면 컴파일러가 자동으로 그 메서드를 사용하도록 코드를 생성함.

명시적으로 타입 매개변수를 지정했다면 효율적으로 동작하는 메서드가 추가 되더라도 기존 제네릭 메서드를 사용될 수밖에 없음

사설:  런타임에서 내부에서 타입을 확인해서 할 수 있겠지만(아이템 19: 런타임에 타입을 확인하여 최적의 알고리즘을 사용하라) 명시적으로 사용하지 않았다면 런타임에 타입을 확인하고 하는 작업이 필요 없어 보임







