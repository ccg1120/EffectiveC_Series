# Effective c# 


# 아이템 37: 쿼리를 사용할 때는 즉시 평가보다 지연 평가가 낫다.

쿼리를 정의한다고 해서 결과 데이터나 시퀀스를 즉각적으로 얻어오는 것은 아님.  
절차만 정의하는 것에 지나지 않음.

쿼리의 결과를 이용하여 순회를 수행해야만 결과가 생성됨 -> 지연 평가

즉각적으로 그 값을 얻어오는 방법 -> 즉시 평가


```c#

private static IEnumerable<TResult> Generate<TResult>(int num, Func<TResult> generator)
{
    for(int index = 0; index < num; ++index)
    {
        yield return generator();
    }
}

private static void LazyEvaluation()
{
    Console.WriteLine($"Start time for Test One :{DateTime.Now:T}");
    var sequence = Generate(10, () => DateTime.Now);

    Console.WriteLine($"waiting press return");
    Console.ReadLine();

    Console.WriteLine("Iterating....");
    foreach (var value in sequence)
        Console.WriteLine($"{value:T}");

    Console.WriteLine($"waiting press return");
    Console.ReadLine();

    Console.WriteLine("Iterating....");
    foreach (var value in sequence)
        Console.WriteLine($"{value:T}");
}
```

```
// 사진 넣기 귀찮아서 텍스트 복붙
Start time for Test One :오전 9:54:10
waiting press return

Iterating....
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
오전 9:54:14
waiting press return

Iterating....
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
오전 9:54:23
```

서로 매번 순회할 때 마다 서로 다른 시간 정보가 출력되는 사실을 확인 할 수 있음.

시퀀스는 값 자체를 갖고 있는 것이 아니라 개별 요소들을 생성하는 방법을 나타내는 코드를 갖고 있음.

```c#
var seq1 = Generate(10, () => DateTime.Now);
var seq2 = from value in seq1
           select value.ToUniversalTime();
```
seq1이 이미 생성해둔 값을 순회 하는것이 아니라 순회 시점에 맞춰 seq1에서 지정한 코드를 호출 하여 그 결과를 사용함

전체 시퀀스가 준비되어야 하는 경우가 있음. 

어떤 경우가 그런지 알고 있다면 성능이 저하 되지 않도록 쿼리를 작성 하는 데 도움이 됨. 기존에 작성한 쿼리로 부터 병목 현상의 원인을 찾을 때도 도움이 됨.

```c#
static IEnumerable<int> AllNumber()
{
    var number = 0;
    while(number < int.MaxValue)
    {
        yield return number++;
    }
}

static void Main(string[] args)
{
    var answers = from number in AllNumber()
                    select number;

    var smallNumbers = answers.Take(10);
    foreach (var num in smallNumbers)
        Console.WriteLine(num);
}
```

전체 시퀀스가 필요하지 않은 경우를 보여 주기 위한 코드 


```c#
static void Main(string[] args)
{
    var answers = from number in AllNumber()
                    where number < 10
                    select number;

    var smallNumbers = answers.Take(10);
    foreach (var num in smallNumbers)
        Console.WriteLine(num);
}
```

위 코드는 동일한 작업을 하지만 전체 시퀀스가 필요한 예임

전체 시퀀스가 필요한 메서드를 사용할 때는 염두해야할 사항이 있음

1. 무한정 지속될 가능성이 있다면 이 같은 메서드를 사용 할 수 없음
2. 필터링하는 쿼리 메서드는 다른 쿼리보다 먼저 수행하는 것이 좋음(쿼리 성능 개선)

```c#
//정렬 후 필터링
var sortedProductsSlow =    from p in products 
                            orderby p.UnitsInStock descending
                            where p.UnitsInStock > 100
                            select p;
//필터링 후 정렬
var sortedProductsFase =    from p in products 
                            where p.UnitsInStock > 100
                            orderby p.UnitsInStock descending
                            select p;
```

대부분의 경우 지연 평가가 더 나은 접근 방식임이 분명함.
즉각적으로 평가가 필요한 경우 ToList(), ToArray() 메서드를 사용하면 됨

두 메서드는 2가지 경우 유용함 
1. 실제 순회 하지 전 데이터의 스냅샷을 얻고자 하는 경우
2. 동일한 결과를 반환해야하는 경우

즉시 평가가 반드시 필요한 경우가 아니라면 대체로 지연 평가를 사용하는 편이 훨씬 낫음