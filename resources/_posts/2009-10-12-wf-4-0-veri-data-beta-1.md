---
layout: post
title: "WF 4.0 - Veri(Data)[Beta 1]"
date: 2009-10-12 14:48:00 +0300
description: "Bir süredir Workflow Foundation 4.0 ile ilişkili blog yazılarını, makaleleri ve görsel dersleri takip etmekteyim. Bu araştırmalarım sırasında Workflow Foundation 4.0 modelinde veriye(Data) olan bakış açısının WF 3.X sürümüne göre oldukça farklılaştığını gördüm."
categories:
  - wf-4-0-beta-1
tags:
  - wf-4-0-beta-1
  - csharp
  - dotnet
  - workflow-foundation
  - wpf
  - generics
  - visual-studio
  - dependency-management
---
# WF 4.0 - Veri(Data)[Beta 1]
Merhaba Arkadaşlar,

Bir süredir Workflow Foundation 4.0 ile ilişkili blog yazılarını, makaleleri ve görsel dersleri takip etmekteyim. Bu araştırmalarım sırasında Workflow Foundation 4.0 modelinde veriye (Data) olan bakış açısının WF 3.X sürümüne göre oldukça farklılaştığını gördüm. WF 3.X tabanlı modelde, aktivite bazlı verileri temsil etmek için genellikle standart sınıf özelliklerinden (Property) veya WPF'ten esinlenilen bağımlı özelliklerden (Dependency Property) yararlanılmaktadır. WF 4.0 modelinde ise veriyi temsil etmek amacıyla Variable veya Argument türevli tiplerden yararlanıldığı görülmektedir.

Bu tiplerin türevleri veriyi doğrudan tutmazlar. Bunun yerine veriyi tanımlar ve elde edilmesini sağlarlar. Verinin içeriği Workflow üzerinde bir yerlerde saklanmaktadır. Bu noktada Variable kavramını aynen Imperative programlama dillerindeki kullanılış biçimi ile düşünebiliriz. Bu nedenle Variable veya Argument türevli tipler tanımlandıkları scope dahilinde kullanılabilirler. Dolayısıyla bir Variable bir Workflow için kök seviyede (Root Level) tanımlanırsa tüm alt aktiviteler tarafından kullanılabilir. Bugünkü örneğimizde [bir önceki blog](https://www.buraksenyurt.com/post/WF-40-Workflow-ve-Object-Initialization.aspx)yazımızda olduğu gibi kod bazlı bir Workflow örneği geliştirecek ve Variable kullanımı ile veriyi nasıl ele alacağımızı görmeye çalışacağız. Aslında.Net 4.0 içerisindeki oluşuma baktığımızda Variable için Variable isimli bir abstract sınıftan türetme yapıldığını görebiliriz. Aşağıdaki sınıf diyagramında bu durum görülmektedir.

![blg89_ClassDiagram.gif](/assets/images/2009/blg89_ClassDiagram.gif)

Burada dikkat edilmesi gereken noktalardan birisi, Variable tipinin sealed olarak tanımlanmış olmasıdır. Yani kendisinden türetme yapamayız. Diğer yandan Variable tipi abstract bir sınıftır ve bu nedenle kendisinden türetme yapılarak özel Variable türevlerinin üretilmesi mümkündür.

Evet bu kadar laf kalabalığından sonra yeni veri modeline kısaca bakmaya çalışalım. Bu amaçla Visual Studio 2010 Beta 1 üzerinden açtığımız basit bir Console projesini kullanıyor olacağız. Projede Workflow alt yapısını ele almak için tek yapmamız gereken System.Activities isimli assembly'ın referans edilmesi olacaktır.

![blg89_ActivityReference.gif](/assets/images/2009/blg89_ActivityReference.gif)

Uygulama kodlarını ise aşağıdaki gibi geliştirdiğimizi düşünebiliriz.

```csharp
using System;
using System.Activities;
using System.Activities.Statements;

namespace UsingVariables
{
    class Program
    {
        static void Main(string[] args)
        {
            // Değişken tanımlanır. Tanımlanırken varsayılan olarak(Default) bir nesne örneğide verilir
            Variable<Player> firstPlayer = new Variable<Player>
            {
                Name = "FirstPlayer",
                Default = new Player { Name = "Zen-R2", Location = new Location { X = 10, Y = 12, Altitude = 100 }, PlayerType = PlayerType.Computer, TotalPoint=10 }               
            };

            // Bir Sequence aktivitesi tanımlanır. Root aktivite.
            Sequence gameFlow = new Sequence();
            // Variable Sequence nesne örneğinin Variables koleksiyonuna eklenir.
            gameFlow.Variables.Add(firstPlayer);

            // Atama işlemi için basit bir Assign aktivitesi gameFlow isimli Sequence nesne örneğinin Activities koleksiyonuna eklenir.
            // To kısmında kime atama yapılacağı belirlenir. Tasarım zamanından farklı olarak bir expression kullanılır. firstPlayer isimli değişkenin işaret ettiği veri alanında TotalPoint değeri alınır.
            // Value özelliğine verilen değer ile atama yapılır. Atamada o anki variable' ın TotalPoint değeri 10.1 birim arttırılmaktadır.
            gameFlow.Activities.Add(
                new Assign<double>() { 
                     To=new OutArgument<double>(v=>firstPlayer.Get(v).TotalPoint),
                     Value=new InArgument<double>(v=>firstPlayer.Get(v).TotalPoint+10.1)
                }
                );

            // Örnek olarak birde Location özelliğinin işaret ettiği içerik değiştirilmektedir.
            gameFlow.Activities.Add(
                new Assign<Location>()
                {
                    To = new OutArgument<Location>(v => firstPlayer.Get(v).Location),
                    Value = new Location { X = 10, Y = 15, Altitude = 99 }
                }
                );

            // Ekrana bilgi yazdırmak için WriteLine tipinden bir aktivite daha eklenir
            // Text özelliğinde ekrana bilgi yazdırabilmek için InArgument nesne örneğinden yararlanılır ve o anki firstPlayer variable' ının Name ve TotalPoint değerleri yazdırılır.
            gameFlow.Activities.Add(
                new WriteLine { 
                     Text=new InArgument<string>(v=>String.Format("{0} isimli oyuncunun puanı {1}. Deniz seviyesinden yüksekliği {2}",firstPlayer.Get(v).Name,firstPlayer.Get(v).TotalPoint.ToString(),firstPlayer.Get(v).Location.Altitude))
                }
                );
            
            // gameFlow isimli Workflow çağırılır ve başlatılır
            WorkflowInvoker.Invoke(gameFlow);
        }
    }

    class Player
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public PlayerType PlayerType { get; set; }
        public double TotalPoint { get; set; }
    }

    class Location
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Altitude { get; set; }
    }

    enum PlayerType
    {
        Computer,
        Human,
        Hybrid
    }
}
```

Örneğin herhangibir gerçek hayat işlevselliği bulunmamaktadır ancak kullanıcı tanımlı bir tipten (örneğimizde Player isimli sınıf) oluşturulan Variable'ın Sequence tipinden bir nesne örneği içerisinde nasıl değerlendirilebildiği açık bir şekilde görülmektedir. Aslında konsept son derece basittir. Variable tipinden bir nesne örneği tanımlanırken Default özelliği ile ilk değer ataması yapılmaktadır ki zorunlu değildir. Yani Variable'ın içeriği akışa dışarıdan gelebilir. Sonrasında bu değişkenin kullanılmak istenen scope içerisine bildirilmesi gerekmektedir. Örnekte gameFlow isimli Sequence aktivite örneğinin Variables koleksiyonuna yapılan ekleme ile bu bildirim gerçekleştirilmektedir. Nitekim söz konusu değişkeninin, içeride yer alan tüm aktiviteler tarafından kullanılması istenmektedir.

Kodun ilerleyen kısımlarında sembolik olarak firstPlayer isimli Variable tipine ait özelliklerin bazılarında değişiklikler yapılmıştır. Bu işlemler için Expression'lardan yararlanılmaktadır. Değiştirme işlemlerinde dikkat edileceği üzere Assign isimli aktiviteden yararlanılmaktadır. İlk Assign aktivitesine ait To ve Value özelliklerinde InArgument ve OutArgument tiplerinden yararlanılarak verinin elde edilmesi ve değer ataması işlemleri yapılmaktadır.

İkinci Assign aktivitesinde ise generic olarak Location tipi belirtildiğinden Value kısmında doğrudan yeni bir Location nesne örneğine atama yapılmaktadır. Her iki aktivite içinde dikkat edilmesi gereken noktalardan birisi, değeri alınmak istenen özelliğe erişilirken firstPlayer isimli değişkenin Get metodundan yararlanılıyor olmasıdır. Zaten kodlama sırasında intelli-sense özelliği devreye girmekte ve Get metodu sonrasında kullanılabilecek tüm Player tipi özellikleri gösterilmektedir. Programda son olarak Workflow örneğinin çalıştırılması sağlanmaktadır. Uygulamanın çalışma zamanı görüntüsü aşağıdaki gibi olacaktır.

![blg89_WfRuntime.gif](/assets/images/2009/blg89_WfRuntime.gif)

Dikkat edileceği üzere başlangıçta 10 olan puan 10.1 birim arttırılmış ve Location tipi üzerinde tutulan Altitude değeride 1 birim azaltılmıştır.

Elbette Workflow 4.0' ın WPF tabanlı bir IDE kullanıyor olması göz ardı edilemez. Bir başka deyişle söz konusu Variable ekleme işlemleri aslında tasarım zamanında çok daha kolay bir şekilde gerçekleştirilebilmektedir. Ancak yazıyı hazırladığım zaman diliminde kullandığım ve public olan Visual Studio 2010 Beta 1 sürümüne ait WF designer ne yazıkki IDE tekrardan başlatılmasına nedenl olmaktadır.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu nedenle şimdilik kod tarafı ile idare etmeniz gerekiyor.

![Wink](/assets/images/2009/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[UsingVariablesV2.rar (26,44 kb)](/assets/files/2009/UsingVariablesV2.rar)