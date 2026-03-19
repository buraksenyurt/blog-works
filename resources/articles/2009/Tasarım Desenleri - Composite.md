---
title: "Tasarım Desenleri - Composite"
pubDate: 2009-07-12 07:00:00
categories:
  - Tasarım Kalıpları(Design Patterns)
tags:
  - Tasarım Kalıpları(Design Patterns)
  - C#
  - WPF
  - XML
  - Generics
---

# Tasarım Desenleri - Composite
![blg45_4.jpg](images/blg45_4.jpg)

Merhaba Arkadaşlar,

Küçüklüğümde son derece şanslı bir çocuktum. Uzun yıllar Almanya'da çalışan rahmetli babam ve annemin pek çok arkadaşı bana Lego oyuncaklarından göndermiştir. Evde günümün büyük bir çoğunluğunu bu legolar ile oynarak geçirir ve okul zamanında derslerimden geri kalırdım.

Lego oyuncakları zaman içerisinde öylesine geliştiki, artık efsane haline gelen pek çok filmin (Starwars, Indiana Jones vb...) konseptini içerdiğini görmeye başladık. Şimdi bunun konumuz ile ne alakası var diye düşünüyorum. Hemen aralarında rütbe ilişkisi olan legolardan oluşan bir orduyu gözümde canlandırıyorum. Generalden en alt kademedeki ere kadar kadar pek çok rütbe yer alıyor.

Aslında bu organizasyonda yer alan bireylerin hepsi birer asker. Rütbeleri farklı bile olsa. Önemli olan detaylardan birisi bunların aralarındaki organizasyonel ilişkinin aslında bir ağaç yapısı (Tree) şeklinde ifade edilebiliyor olması. Bu ağaç ilişkisinden yararlanarak alt ve üstler arasında bilgi dolaştırılmasıda mümkün. Peki ya bu askerler nesne yönelimli (Object Oriented) bir programlama ortamında ifade ediliyorlarsa, organizasyonun ağaç yapısını temsil edebilecek bir kalıp mümkün olabilir mi? Tabiki olabilir ve bu kalıbın adı Composite tasarım desenidir. Aslında bu desenin temel amacı, nesnelerin ağaç yapısına göre düzenelenebilmesidir. Desenin içerisinde yer alan kahramanlar ise aşağıdaki örnek UML diagramında görüldüğü gibidir.

![blg45_uml.gif](images/blg45_uml.gif)

Component tipi içerisinde kendisinden türeyen Composite ve Leaf tiplerinin ortaklaşa kullanacağı üyeler dışında, ezmeleri gereken kurallarda tanımlanmaktadır. Bu anlamda Component bileşenini abstract bir sınıf veya arayüz (Interface) olarak tanımlayabiliriz. Şemadanda görüleceği üzere, Component tipi içerisinde yine Component tipinden parametre alıp ekleme ve çıkarma işlevlerini üstlenen ve ezilen (Override) metodlar vardır. Ancak bu metodlar Leaf tipi içerisinde ezilmemiştir. Aslında Leaf tiplerini, kendi içerisinde başka bir Component tipi içermeyecek çalışma zamanı nesnelerini örneklediğini düşünebiliriz. Bunun aksine Composite tipi, kendi içerisinde Component tipinden oluşan bir koleksiyon içermektedir. Bir başka deyişle, altında Leaf tiplerini veya başka Composite tipleri içerebilecek bir nesne üretimi sağlanabilmektedir.

Desende dikkat edilmesi gereken noktalardan biriside, Component tipinden tanımlanan bazı operasyonların Composite tipte uygulanırken Leaf tipinde uygulanmamasıdır. Örneğe göre AddSoldier ve RemoveSoldier metodlarının Leaf tipi içerisinde bir anlamı yoktur. Nitekim Leaf kendi altında başka bir Component tipi içermeyecek nesne örneklerini temsil etmek üzere ele alınmalıdır. Diğer yandan Component tipi içerisinde tanımlanıp hem Composite hemde Leaf tipinde uygulanan ortak operasyonların, Composite içerisindeki uygulanış şeklide biraz farklıdır. Bu farka göre, Composite içerisindeki ortak operasyon (örneğimize göre ExecuteOrder metodu), Composite nesne örneğine bağlı tüm nesneleri kapsamalıdır. Sanırım kafamız iyice allak bullak oldu.

![Undecided](images/smiley-undecided.gif)

Bu nedenle olayı XML ağaçlarını düşünerektende ele alabiliriz. XML alt yapısını kod tarafında ifade ederken, Composite tasarım kalıbına göre bir ağacın OOP'ye uygun olacak şekilde tasarlanması kolay olabilir. XML yapısı gereği, herkes birer element (yada node) olarak ifade edilebilirken, aslında kendilerine bağlı başka alt elementleride barındırabilirler. Böylece çalışma zamanında birbirlerine bağlı dallar üzerinde oturan XML ağaçları kolaylıkla tasarlanabilir. Hatta bir Component üzerinden varsa alt veya üst Component tipinede ulaşılabilir. (Size tavsiyem bir XML verisinin içeriğindeki elementleri ifade edecek bir modeli, Composite desenine göre tasarlamaya çalışmanız olacaktır.)

Ancak yapısal (Stuctural) desenelerden olan bu kalıpta dikkat edilmesi gereken en önemli nokta, ağaç içerisindeki tüm nesnelerin aslında aynı arayüzü (veya soyut tipi) uyguluyor olmasıdır. Bu nedenle nesne istemcileri, ağaçta yer alan Composite ve Leaf örneklerine aynı şekilde davranırlar.

Dilerseniz basit bir örnek üzerinden ilerleyelim. Örneğimizde, kurduğumuz ordunun içerisindeki organizasyonel ağacı tasarlamaya çalışıyor olacağız. Buna göre

General
Colonel
LieutenantColonel
Major
Captain
Lieutenant

şeklinde bir organizasyonumuz olduğunu göz önüne alabiliriz.

Organizasyondaki herkes bir askerdir (Soldier) ki buda bizim Component tipimiz ile ifade edilmektedir. Composite tipimiz (CompositeSoldier) isterse kendi içerisinde birden fazla başka Component (PrimitiveSoldier veya CompositeSoldier olabilir) tiplerini içerebilmelidir. Tüm askerlerin, ister Leaf ister Composite olsun uygulayacağı birde ortak operasyonumuz vardır (ExcuteOrder). İşte sınıf diagramımız ve uygulama kodlarımız.

![blg45_6.gif](images/blg45_6.gif)

```csharp
using System;
using System.Collections.Generic;

namespace CompositePattern
{
 /// <summary>
 /// Askerlerin rütbeleri
 /// </summary>
 enum Rank
 {
  General,
  Colonel,
  LieutenantColonel,
  Major,
  Captain,
  Lieutenant
 }
 
 /// <summary>
 /// Component sınıfı
 /// </summary>
 abstract class Soldier
 {
  protected string _name;
  protected Rank _rank;
  
  public Soldier(string name,Rank rank)
  {
   _name=name;
   _rank=rank;
  }
  
  public abstract void AddSoldier(Soldier soldier);
  public abstract void RemoveSoldier(Soldier soldier);  
  public abstract void ExecuteOrder(); // Hem Leaf hemde Composite tipi için uygulanacak olan fonksiyon
   
 }
 
 /// <summary>
 /// Leaf class
 /// </summary>
 class PrimitiveSoldier
  :Soldier{
  
  public PrimitiveSoldier(string name,Rank rank)
   :base(name,rank)
  {
   
  }
  // Bu fonksiyonun Leaf için anlamı yoktur.
  public override void AddSoldier(Soldier soldier)
  {
   throw new NotImplementedException();
  }
  // Bu fonksiyonun Leaf için anlamı yoktur.
  public override void RemoveSoldier(Soldier soldier)
  {
   throw new NotImplementedException();
  }
  
  public override void ExecuteOrder()
  {
   Console.WriteLine(String.Format("{0} {1}",_rank,_name));
  }  
 }
 
 /// <summary>
 /// Composite Class
 /// </summary>
 class CompositeSoldier
  :Soldier{
 
  // Composite tip kendi içerisinde birden fazla Component tipi içerebilir. Bu tipleri bir koleksiyon içerisinde tutabilir.
  private List<Soldier> _soldiers=new List<Soldier>();
  
  public CompositeSoldier(string name,Rank rank)
   :base(name,rank)
  {
   
  }
  
  // Composite tipin altına bir Component eklemek için kullanılır
  public override void AddSoldier(Soldier soldier)
  {
   _soldiers.Add(soldier);
  }
  // Composite tipin altındaki koleksiyon içerisinden bir Component tipinin çıkartmak için kullanılır
  public override void RemoveSoldier(Soldier soldier)
  {
   _soldiers.Remove(soldier);
  }  
  // Önemli nokta. Composite tip içerisindeki bu operasyon, Composite tipe bağlı tüm Component'ler için gerçekleştirilir.
  public override void ExecuteOrder()
  {
   Console.WriteLine(String.Format("{0} {1}",_rank,_name));
   foreach(Soldier soldier in _soldiers)
   {
    soldier.ExecuteOrder();
   }
  }
 }
 class Program
 {
  public static void Main(string[] args)
  {
   // Root oluşturulur.   
   CompositeSoldier generalBurak=new CompositeSoldier("Burak",Rank.General);
   
   // root altına Leaf tipten nesne örnekleri eklenir.
   generalBurak.AddSoldier(new PrimitiveSoldier("Mayk",Rank.Colonel));
   generalBurak.AddSoldier(new PrimitiveSoldier("Tobiassen",Rank.Colonel));
   
   // Composite tipler oluşturulur.
   CompositeSoldier colonelNevi=new CompositeSoldier("Nevi", Rank.Colonel);   
   CompositeSoldier lieutenantColonelZing=new CompositeSoldier("Zing", Rank.LieutenantColonel);
   
   // Composite tipe bağlı primitive tipler oluşturulur.
   lieutenantColonelZing.AddSoldier(new PrimitiveSoldier("Tomasson", Rank.Captain));
   colonelNevi.AddSoldier(lieutenantColonelZing);
   colonelNevi.AddSoldier(new PrimitiveSoldier("Mayro", Rank.LieutenantColonel));
   // Root' un altına Composite nesne örneği eklenir.
   generalBurak.AddSoldier(colonelNevi);
   
   // 
   generalBurak.AddSoldier(new PrimitiveSoldier("Zulu",Rank.Colonel));
   
   // root için ExecuteOrder operasyonu uygulanır. Buna göre root altındaki tüm nesneler için bu operasyon uygulanır
   generalBurak.ExecuteOrder();
   
   Console.ReadLine();
  }
 }
}
```

Uygulamayı çalıştırdığımızda aşağıdaki sonucu alırız.

![blg45_7.gif](images/blg45_7.gif)

Görüldüğü gibi emir modeli general üzerinden uygulandığı için organizasyonda generale bağlı olan herkese iletilebilmektedir. Aslında son derece kolay ve kullanım alanı geniş olan bir deseni inceledik. Ancak Console uygulamasıda olsa eksik olan kısımlar var gibi. Söz gelimi hiyerarşiye göre askerlerin ekrana girintili olarak yazdırılması sağlanabilir.

![Wink](images/smiley-wink.gif)

Hatta bunu bir WPF veya Windows uygulamasında görsel olarak yapmaya çalışmanızı öneririm. Bu mukaddes görevleride sizlere bırakıyorum. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Not: Desene ait görsel anlatım en yakın zamanda eklenecektir.

[CompositePattern.rar (13,44 kb)](assets/CompositePattern.rar)
