---
layout: post
title: "C# 6.0 - Collection Initializers için Add Metodunu Yönlendirmek"
date: 2017-03-18 22:50:00 +0300
description: "C# 6.0 ile gelen yeni özelliklerden birisi de geliştirici tanımlı koleksiyonlar içerisindeki üyeler için Collection Initializers kullanılabilmesi. Bunun için Add genişletme metodunun istenen metoda atanması yeterli. Konuyu daha iyi anlamak için C# 6.0 öncesinden bir örnekle işe başlayalım. Elimizde aşağıdaki gibi bir sınıf olduğunu düşünelim."
categories:
  - csharp-6-0
tags:
  - csharp-6-0
  - csharp
  - dotnet
  - generics
  - visual-studio
---
# C# 6.0 - Collection Initializers için Add Metodunu Yönlendirmek
![lego_collection_n.gif](/assets/images/2017/lego_collection_n.gif)

Merhaba Arkadaşlar,

Mesleki hayatımın bir döneminde yazılım eğitmeni olarak çalıştım. Ağırlıklı olarak.Net eğitimleri verdim. Tabii o zamanlar.Net nispeten daha kolaydı. Bu kadar fazla dallanan bir Framework değildi ama C# dil özellikleri de acımasızca genişlemiyordu. Aradan geçen onca yıldan sonra çalıştığım turuncu bankanın kendi akademisinden iç eğitim isteği geldi.

Konu C# programlama diliydi. İyi hazırlanmam gerektiği ortadaydı. Kurumsal projelerin standartları gereği en son teknolojiyi kullanamıyor olsak da C# 6.0 dünyasını yakalamıştık. Projelerimizi Visual Studio 2013 ile geliştirmekte ve.Net Framework 4.6.1 sürümünü kullanmaktayız (Şimdilik) Bana sanal bir makine de verdiler ve içerisine gıcır gıcır Visual Studio 2015 koydular (Kurum içinde de Visual Studio 2015 yaygınlaştırılmakta)

Hal böyle olunca C# 6.0 ile ilgili yenilikleri gözden geçirme fırsatını da yeniden yakalamış oldum. Eğitime hazırlanırken okuduğum özelliklerden birisi de Collection Initializer olarak kullanılabilen Add metodunun istediğimiz bir başka metoda atanabilmesiydi. Bu, özellikle bir koleksiyonu sarmalladığımız IEnumerable türevli tiplerde işe yarayabilecek bir yetenek. Amacımız bir sınıf örneğini oluşturduğumuz yerde içerideki koleksiyona elemanlar atayabilmek. Bunu zaten var olan koleksiyonlar için aşağıdaki gibi yapabiliyoruz.

```csharp
List<string> colors=new List<string>{
			"red",
			"blue",
			"green"
	   };
```

Örneğin string tipte elemanlar barındıran colors isimli List nesnesini yukarıdaki kod parçasında olduğu gibi başlatabiliriz. Nesne oluşurken içerisindeki string dizisine ilk elemanlar atanıyor.

> Hatta C# 6.0 ile Dictionary tipinden koleksiyonları aşağıdaki gibi başlatabiliyoruz da (Kod Visual Studio 2015 de yazılmıştır)
> ```csharp
> var colors = new Dictionary<int, string>
> {
> 	[100] = "Black",
> 	[101] = "Green",
> 	[102] = "Red"
> };
> ```

Hedefimiz string yerine kendi tipimizi kullanıldığında benzer işlevselliği sunabilmek. Add metodunun olması bunun için yeterli ama bulunduğumuz domain gereği farklı isimli bir metoda atamak isteyebiliriz? İşte soru bu. Konuyu daha iyi anlamak için C# 6.0 öncesinden bir örnekle işe başlayalım. Elimizde aşağıdaki gibi bir sınıf olduğunu düşünelim (Bu örnek Visual Studio 2013 üzerinde geliştirildi)

```csharp
public class Player
{
	public int PlayerId { get; set; }
	public string Nickname { get; set; }
	public double Level { get; set; }

	public override string ToString()
	{
		return string.Format("{0} {1} {2}", PlayerId, Nickname, Level);
	}
}
```

Kobay sınıflarımızdan birisi olan Player tipinden generic List koleksiyonu barındıran bir diğer tipimizi de aşağıdaki gibi tasarlayalım.

```csharp
public class GameSchene
	:IEnumerable<Player>
{
	private List<Player> players = new List<Player>();

	public void Assign(Player player)
	{
		players.Add(player);
	}

	public IEnumerator<Player> GetEnumerator()
	{
		return ((IEnumerable<Player>)players).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<Player>)players).GetEnumerator();
	}
}
```

GameSchene sınıfı IEnumerable arayüzünü uygulamakta. Buna göre içerideki oyuncu listesi üzerinden for each döngüsü yardımıyla ilerleyebiliriz. Amacımız ise aşağıdaki gibi bir kod parçasını çalıştırabilmek.

```text
class Program
{
	static void Main(string[] args)
	{
		var zone = new GameSchene
		{
			new Player{ PlayerId=102, Nickname="Maykil Cordin", Level=1000},
			new Player{ PlayerId=104, Nickname="a user", Level=800},
			new Player{ PlayerId=89, Nickname="black window", Level=750}
		};

		foreach (var p in zone)
		{
			Console.WriteLine(p.ToString());
		}
	}
}
```

Aslında string tipinden elemanlar barındıran generic bir kolekisyonu tek ifade içerisinde nasıl başlatabiliyorsak, benzer durumu kendi koleksiyon tipimiz için de yapmak istiyoruz. Ancak kod derlenmeyecek. Nitekim koleksiyon başlatıcısı Add metodunu bekliyor. Örnekte ise Assign metodu var.

![c6extension_1.gif](/assets/images/2017/c6extension_1.gif)

![c6extension_3.gif](/assets/images/2017/c6extension_3.gif)

Pek tabii Assign metodunu Add olarak değiştirirsek kod başarılı bir şekilde derlenecek ve çalışacaktır. C# 6.0 tarafında ise Add metodunu extension olarak tanımlayıp istediğimiz başka bir metod ile ilişkilendirme şansına sahibiz. Tek yapmamız gereken aşağıdaki gibi bir genişletme sınıfı yazmak.

```csharp
public static class GameScheneExtensions
{
	public static void Add(this GameSchene e, Player p) => e.Assign(p);
}
```

GameSchene tipi için Add isimli bir genişletme metodu söz konusudur. Expression-Bodied Functions özelliği kullanılarak => operatörü sonrası işletilmesi istenen kod tanımlanmıştır. e.Assing (p) ile Add metodunun güncel GameSchene nesne örneğindeki Assing metoduna atanması ve initialization kısmında gelen Player nesne örneğinin parametre olarak atanması sağlanmıştır. Çalışma zamanı çıktısı aşağıdaki gibidir (Örnek Visual Studio 2015 üzerinde yazılmıştır)

![c6extension_2.gif](/assets/images/2017/c6extension_2.gif)

Basit ve ince bir özellik bilgimiz olsun mutlaka işimize yarayacaktır. Böylece geldik bir yazımızın daha sonuna. Bir başka yazımızda görüşünceye dek hepinize mutlu günler dilerim.