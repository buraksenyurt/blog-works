---
layout: post
title: "Workflow Foundation 4.0 - Custom Activity Geliştirmek [Beta 2]"
date: 2010-01-01 07:00:00 +0300
description: "Programlamaya profesyonel olarak adım attığım yıllarda henüz .Net mimarisi geliştirilmeden önce Delphi programlama dili ile ürünler yazmaya çalışırdım. Aslında .Net çıktığından beri uğraşmadığım Delphi programlama dilini düşündüğümde aklıma ilk gelen hızlı geliştirme(Rapid Development) için sunduğu zengin Component sekmeleridir."
categories:
  - wf-4-0-beta-2
tags:
  - wf-4-0-beta-2
  - csharp
  - xml
  - dotnet
  - aspnet
  - workflow-foundation
  - xaml
  - authentication
  - java
  - javascript
  - generics
  - visual-studio
  - datatable
---
# Workflow Foundation 4.0 - Custom Activity Geliştirmek [Beta 2]
![blg108_Giris.jpg](/assets/images/2010/blg108_Giris.jpg)

Merhaba Arkadaşlar,

Programlamaya profesyonel olarak adım attığım yıllarda henüz.Net mimarisi geliştirilmeden önce Delphi programlama dili ile ürünler yazmaya çalışırdım. Aslında.Net çıktığından beri uğraşmadığım Delphi programlama dilini düşündüğümde aklıma ilk gelen hızlı geliştirme (Rapid Development) için sunduğu zengin Component sekmeleridir. Sayısız bileşen sayesinde müşteri ihtiyaçlarına çok hızlı cevap verebilecek ürünler geliştirebildiğimi de gayet net hatırlayabiliyorum. Tabi.Net dünyasına geçiş yapıp Visual Studio.Net (2005,2008,2010) ortamı ile karşılaştırıldığında çok fazla bileşen olduğu göze çarpmaktaydı. Ancak.Net tarafında da kodlama yeteneklerinin daha ön plana çıktığı bir gerçekti.

Ancak ister Delphi olsun ister.Net ister Java platformu, görsel program geliştirme ortamlarında var olan bileşenlerin yeterli gelmediği durumlar ile karşılaşılabilir. Bu durumda, geliştiriciler kendi bileşenlerini yazmaya çalışırlar (Yada ihtiyaçlarını karşılayan bileşenleri satın alma yoluna giderler

![Wink](/assets/images/2010/smiley-wink.gif)

). Bunun pek çok nedeni olabilir. Bu nedenler basit Web tabanlı bir TextBox kontrolünün güvenlik açığı, çok sık kullanılan IPV4 girişleri için bir kontrolün bulunmayışı veya birden fazla kontrolün birleşiminin pek çok yerde kullanılması gerekliliği vb olabilir...

Özel bileşenlerin geliştirilmesi genellikle bellidir. Bunlardan birisi bileşenin sıfırdan yazılmasıdır. Bu zaman zaman zahmetli bir yoldur. Söz gelimi Windows veya Web tarafında ekrana basılacak olan çıktının üretilmesi için gerekli kodlamaların yapılması gerekir. Bu Windows tarafında Paint olayının ezilerek GDI/DirectX gibi tekniklerin konuşturulması veya Web tarafında HTML ve Javascript destekli kodlamaların yapılmasını gerektirebilir. İkinci bir yol var olan bileşenlerin tasarım ortamında bir arada değerlendirilerek composite üretimlerin gerçekleştirilmesidir. Yine Windows ve Web tarafındaki User Control'leri bu anlamda düşünebiliriz. Diğer bir teknik ise, var olan bir bileşenden türetme yoluna gidilerek geliştirmenin yapılmasıdır ki bu çok sıkça tercih edilmektedir. Peki Workflow Foundation alt yapısında kendi bileşenlerimizi nasıl yazabiliriz? Aslında Workflow Foundation tarafında kullanılan Activity'lerin, Windows veya Web tarafındaki görsel olan/olmayan kontrollerden pekte farkı yoktur. Aktivite'lerde Workflow Foundation tarafında geliştirici tarafından değerlendirilen özel bileşenlerdir.

İşte bu yazımızda Workflow Foundation 4.0 alt yapısında, kendi aktivite bileşenlerimizi nasıl geliştirebileceğimizi incelemeye çalışıyor olacağız. An itibariyle Workflow Foundation 4.0 Beta 2 sürümü üzerinden ilerleyeceğiz. Aslında Custom Activity geliştirmek için pek çok neden sıralanabilir. Genel olarak var olan built-in aktivite bileşenlerinin yeterli gelmediği veya özel business logic'lerin içeren aktivitelerin pek çok akışta kullanılması gerektiği durumlarda özel aktivite bileşenlerinden yararlanılabilir. Özel aktiviteleri geliştirmeye başlamadan önce.Net Framework 4.0 Beta 2 bünyesindeki aktivite hiyerarşisini iyi bir şekilde incelemek gerekmektedir.

Genel Aktivite hiyerarşisi;

![blg108_Hierarchy.gif](/assets/images/2010/blg108_Hierarchy.gif)

Burada önemli olan nokta, her aktivitenin bir Activity tipi olması gerekmediğidir. Evet nesne yönelimli teoriye göre böyledir ancak mantıksal çerçevede böyle değildir. Özellikle generic ve normal tip yönelimlerine bakıldığında amaca göre uygun olan aktivite bileşeninden türetilme yoluna gidilmesinin tercih edillmesi gerektiği ortadadır. Bu nedenle doğrudan Activity bileşeni yerine CodeActivity, AsyncCodeActivity, NativeActivity veya bunların generic olan versiyonlarından türetme işlemleri yapılmaktadır. Aslında doğrudan Activity/Activity tipinden türetme yolu da tercih edilebilir. Aslında biraz kafamızın karıştığı ortadadır.

![Undecided](/assets/images/2010/smiley-undecided.gif)

Hangi türetmeyi seçeğimize karar vermek için şu noktalara dikkat etmemiz yeterli olacaktır;

- Diğer aktivitelerin bir arada kullanılmasından ve doğrudan XAML tanımlamalarından oluşturalacak özel aktivite bileşenlerinin (bunları Asp.Net veya Windows uygulamalarındaki User Control'lere benzetebiliriz) Activity/Activity tipinden türetilmesi,
- Özel aktivitenin çalışma zamanında sonlandırılması sırasında bazı kodlamalara ihtiyaç duyuluyorsa CodeActivity,
- Aktivite bileşeninin içerdiği işlemleri asenkron olarak yürütebilmesi isteniyorsa AsyncCodeActivity,
- Eğer özel aktivite bileşeni çalışma zamanı verilerine erişecekse (örneğin diğer aktivitelere ulaşılıp kataloglanması, takvimlendirilmesi-scheduling vb...) NativeActivity,

türevli olması tercih edilmektedir.

Görüldüğü üzere aslında özel aktivite bileşeni geliştirmek için birden fazla yol vardır. Dilerseniz konuyu basit bir örnek ile süsleyerek ilerlemeye çalışalım. Bizim için en basit olanı seçeceğiz. Bu amaçla CodeActivity türevli bir özel bileşen geliştirmeye çalışacağız. Söz konusu bileşeni Workflow Activity Library tipinden olan proje içerisine yeni bir Code Activity öğesi ekleyerek örnekleyebiliriz. AdapterActivity ismiyle geliştireceğimiz bileşenimizin temel amacı Select sorgusu ile bir veritabanı tablosunun içeriğini DataTable tipinden bir referans olarak geriye döndürmektir. AdapterActivity isimli örnek bileşenimizin kod içeriği aşağıdaki gibidir.

![blg108_ClassDiagram.gif](/assets/images/2010/blg108_ClassDiagram.gif)

```csharp
using System;
using System.Activities;
using System.Data;
using System.Data.SqlClient;

namespace ActivityLibrary2
{
    public sealed class AdapterActivity 
        : CodeActivity<DataTable>
    {
        // Aktivite içerisinde kullanılan bazı özellikler
        public InArgument<string> Server { get; set; }
        public InArgument<string> Database { get; set; }
        public InArgument<string> SelectQuery { get; set; }

        // Execute metodu ezilir. AdapterActivity tipi, CodeActivity<DataTable> sınıfından türediğinden Execute metodunun DataTable türünden bir değer döndürmesi sağlanmalıdır
        // Execute metodunun dönüş değeri, Designer tarafında Result isimli özellik ile yakalanabilir
        protected override DataTable Execute(CodeActivityContext context)
        {
            // InArgument<T> tipinden olan değişken değerlerinin çalışma zamanında alınması için Get metodlarından yararlanılır. Metod parametre olarak o anki Aktivite içeriğinin referansını kullanır.
            string conStr = String.Format("data source={0};database={1};integrated security=SSPI", Server.Get(context), Database.Get(context));
            DataTable table=null;
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(SelectQuery.Get(context), conn);
                table = new DataTable();
                adapter.Fill(table);
            }
            return table;            
        }
    }
}
```

CodeActivity türevli olan AdapterActivity bileşeni, bir Select sorgusunu çalıştırıp sonucu DataTable içerisine aktarmak üzere tasarlanmıştır. Çok tabi olarak örneği basit tuttuğumuzu ifade etmek isterim. UserId,Password gibi ConnectionString için önemli olan alanlar yer almamaktadır. Bunun yerine varsayılan olarak Windows doğrulama modunda bağlanılabildiği düşünülmektedir. Üstelik yazılan SQL sorgusunun geçerli olup olmadığına dair bir kontrolde bulunmamaktadır ki olmasında yarar vardır. Nitekim hatalı veya SQL Injection saldırılarına açık tipte bir SQL sorgusunun yazılabiliyor olması arzu edilmeyecektir. Aslında tüm bu iki handikap sadece tablo adının alınıp Select sorgusunun kod içerisinde dinamik olarak oluşturulmasıyla da çözümlenebilir.

Ancak odaklanmamız gereken daha önemli bir mevzu vardır. Override edilimiş olan Execute metodu. Bu metod söz konusu kod aktivitesi çalıştığında işletilecek olan kodları içermektedir. Dikkat edileceği üzere o an üzerinde çalıştığı aktivitenin çalışma zamanı içeriğini kullanabilmesi için CodeActivityContext tipinden bir parametre ile süslenmiştir. Bu parametre kod içerisinde Server, Database ve SelectQuery gibi özelliklerin çalışma zamanı değerlerini almak için çağırılan Get metodlarında parametre olaraktan da kullanılmaktadır. Dikkat çekici bir diğer nokta Execute metodunun dönüş tipidir. Varsayılan olarak void olan Execute metodu, AdapterActivity'nin CodeActivity türevli olması nedeniyle geriye DataTable döndürmektedir. Nitekim generic olarak yapılan türetmeye göre T tipi, aynı zamanda Execute metodunun geri dönüşü tipidir.

Bu basit bileşeni test etmek için aşağıdaki XAML ve tasarım zamanı içeriğine sahip TestScene isimli bir aktivite oluşturduğumuzu düşünebiliriz. (Uzun olan Namespace tanımlamaları göz ardı edilerek sadece Sequence içeriği verilmiştir)

```xml
<Sequence sad:XamlDebuggerXmlReader.FileName="C:\Documents and Settings\bsenyurt\my documents\visual studio 10\Projects\ActivityLibrary2\ActivityLibrary2\TestScene.xaml" sap:VirtualizedContainerService.HintSize="232,245">
    <Sequence.Variables>
      <Variable x:TypeArguments="sd1:DataTable" Name="QueryResult" />
    </Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <local:AdapterActivity Database="AdventureWorks" sap:VirtualizedContainerService.HintSize="210,22" Result="[QueryResult]" SelectQuery="Select * From Production.Product Order By Name desc" Server="." />
    <WriteLine sap:VirtualizedContainerService.HintSize="210,59" Text="[QueryResult.Rows.Count.ToString()]" />
  </Sequence>
```

Tasarım Zamanı (Design Time);

![blg108_Using.gif](/assets/images/2010/blg108_Using.gif)

Tasarım zamanında dikkat edileceği üzere kod tarafında eklediğimiz Database, Server ve SelectQuery gibi özelliklere ulaşılabilmektedir. TestScene aktivitesi QueryResult isimli bir Variable değerine sahiptir. Bu değişkenin içeriği ise AdapterActivity isimli bileşenin Result özelliğidir. Bir başka deyişle CodeActivity türevli özel aktivitenin Execute metoduna ait dönüş değerinin, Result isimli özelliğe aktarıldığını söyleyebiliriz. Gelelim TestScene.xaml aktivitesinin çalıştırılacağı kodlara;

```csharp
using System.Activities;

namespace ActivityLibrary2
{
    public class Program
    {
        static void Main(string[] args)
        {
            var result=WorkflowInvoker.Invoke(new TestScene());
        }
    }
}
```

Görüldüğü gibi tek yaptığımız WorkflowInvoker tipinin Invoke metodu ile yeni bir TestScene aktivite örneğini çalıştırmaktır. Test için AdventureWorks veritabanında yer alan Products tablosunu kullandığımızda, standart olarak 504 olan satır sayısı değerinin elde edilmesi gerekmektedir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![blg108_Runtime.gif](/assets/images/2010/blg108_Runtime.gif)

Custom Activity geliştirmek ile ilişkili diğer teknikleri de ilerleyen yazılarımızda ele almaya çalışıyor olacağız. Söz gelimi aktivitilerin çoğu için tasarım zamanı desteği bulunmaktadır. Kendi geliştireceğimiz aktivite bileşenleri için bu tip destekleri sunabilir miyiz? Sunabilirsek nasıl?

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ActivityLibrary2.rar (47,00 kb)](/assets/files/2010/ActivityLibrary2.rar)