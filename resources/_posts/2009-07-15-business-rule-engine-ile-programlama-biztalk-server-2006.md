---
layout: post
title: "Business Rule Engine ile Programlama(Biztalk Server 2006)"
date: 2009-07-15 13:30:00 +0300
description: "Yıllardır yazılım projelerinde görev almaktayım. Çeşitli projelerde pek çok zorlukla karşılaştım. Özellikle enterprise seviyedeki projelerde karşılaştığım en büyük zorluklar arasında, müşterinin uygulama içerisinde tanımladığı iş kurallarını istediği gibi ve görsel arayüzler yardımıyla değiştirmek istemesi yer almaktaydı..."
categories:
  - biztalk
tags:
  - biztalk
  - csharp
  - dotnet
  - wcf
  - xml
  - xaml
  - http
  - caching
  - testing
  - visual-studio
---
# Business Rule Engine ile Programlama(Biztalk Server 2006)
![blg46_1.jpg](/assets/images/2009/blg46_1.jpg)

Merhaba Arkadaşlar,

Yıllardır yazılım projelerinde görev almaktayım. Çeşitli projelerde pek çok zorlukla karşılaştım. Özellikle enterprise seviyedeki projelerde karşılaştığım en büyük zorluklar arasında, müşterinin uygulama içerisinde tanımladığı iş kurallarını istediği gibi ve görsel arayüzler yardımıyla değiştirmek istemesi yer almaktaydı. Burada geliştirme açısından bakıldığında zafer, söz konusu iş kurallarını uygulama kodu üzerinde herhangibir güncelleme yapılmasına gerek bırakmadan entegre edebilen sistemleri geliştirmektir aslında.

![Wink](/assets/images/2009/smiley-wink.gif)

Tabiki burada müşterinin kastettiği iş kurallarının nasıl tanımlandığından tutunda dile getiriliş şekli daha çok büyük öneme sahiptir. Nitekim öyle kurallar olabilir ki, yada bu kurallar öyle şekillerde dile getirilebilirki, yorumlayabilmek veya uygulatabilmek için yapay zeka stratejilerinin ele alınması zorunlu hale gelir. Ben tabiki konunun bu kısmına en azından şu an için girmemeyi tercih etmekteyim

![Smile](/assets/images/2009/smiley-smile.gif)

Özetle büyük çaplı uygulamalarda karşılaştığımız en büyük sorunlardan birisinin, müşterinin kullandığı ürün ile ilişkili olaraktan tanımlamış olduğu iş kurallarının (Business Rules) koda müdahele etmeden yönetilebilmesi olarak düşünebiliriz. Bazı durumlarda, ürüne ait iş kuralları baştan bellidir ve değişmezdir. Bu tip senaryolara az rastlanmakla birlikte geliştirilmesi kolaydır. Nitekim kod içerisinde konulacak katı kurallar ile söz konusu geliştirme pekala yapılabilir.

Ancak, müşterinin uygulama üzerindeki iş kurallarını yeri geldiğinde değiştirebilmesi isteği (çok sık olmasa bile) geliştirme sürecinde bizleri bir çıkmaza düşürebilir. Öyleki, çalışmakta olan sistem içerisindeki kuralların esnetilebilmesi, değiştirilebilmesi ve hatta yenilerinin eklenebilmesi demek, kodu geliştirmeye devam etmek (Development), yeniden test (Testing) ve tekrardan dağıtım (Deployment) anlamına gelmemelidir. Her geliştirici takımı bu tip durumlara karşın, uygulamasının kodunu tekrardan güncellemeye gerek bırakmadan yeni kuralları kolayca öğrenebilmesi üzerine tasarlamak ister. Lakin bu sanıldığı kadar kolay bir süreç olmayabilir. Bir noktada XML tabanlı olaraktan söz konusu kuralların saklanması ve kod içerisine ele alınması düşünebilir.

Hatta daha önceden çalıştığım çok değerli bir şirketin iş akışları üzerine geliştirdiği bir ürün, akış tasarımları, yönetimi ve geliştirilmesi için XML tabanlı olan ve basit IDE ile çalışan sistemi, Web tabanlı uygulama olarak başarılı bir şekilde dağıtabilmiştir. Hatta Workflow tabanlı WCF servislerinde (Workflow Based WCF Services) bile gelinen nokta, içeriğin XAML olarak ifade edilebilmesi ve bu nedenle koda müdahele etmeden de değiştirilebilmesi değil midir?

![Wink](/assets/images/2009/smiley-wink.gif)

Tüm bunlar bir yana dursun Biztalk ailesinde, kuralları kolayca geliştirebileceğimiz, veritabanı (database), XML veya.NET tipleri gibi kaynaklardan kural verilerini alıp değerlendirebileceğimiz bir IDE zaten mevcuttur (Business Rule Composer). Hatta kendi uygulamalarımız için Biztalk'un hazır kural moturunuda (Business Rule Engine-BRE) kullanabiliriz. Sanıyorum ki artık sadede gelsem iyi olacak. Bu yazımızda giriş seviyesininde altında kalmak üzere, Biztalk Server 2006 ile birlikte gelen Business Rule Engine kütüphanesini nasıl kullanabileceğimizi ve iş kurallarını tanımlamak için Business Rule Composer aracını nasıl ele alabileceğimizi incelemeye çalışıyor olacağız.

Biztalk Server ile birlikte gelen Business Rule Engine'in, kendi geliştirdiğimiz.Net uygulamalarında kullanılabilmesi için, sunucu lisansına sahip ürünün yanlızca Business Rules Components özelliğinin kurulması yeterlidir. Tabiki burada önemli olan noktalardan birisi lisans konusudur. Lisanslı olan bir Biztalk Server ürünü üzerinden kurulum yapılmalıdır. Bu nedenle, kendi uygulamalarımızdan kasıt çoğunlukla sunucu tarafında çalışan servis uygulamalarıdır. Böylece, servis tabanlı.Net uygulamalarımız içerisinde istersek, Biztalk Server ile birlikte gelen kural motorunu kullanabiliriz. Business Rule Engine uzun uzun yıllar önce (1974) geliştirilmiş [RETE](http://en.wikipedia.org/wiki/Rete_algorithm)algoritmasını kullanmaktadır. Tabi başlamadan önce önem arz eden bazı kavramlardan bahsetmekte yarar olduğu kanısındayım. Bunlar;

![blg46_4.jpg](/assets/images/2009/blg46_4.jpg)Business Rule Composer: İlkeleri (Policy), içerisindeki kuralları (Rules) ve daha fazlasını tasarlayabileceğimiz bir arabirim olarak düşünülebilir. Kısaca iş kurallarını görsel olarak oluşturduğumuz programdır.

Policy: İçinde, iş kurallarını barındıran nesnedir. Bu nesne istenildiğinde versiyonlandırılabilir. Bu sayede, aynı policy'nin farklı kurallar içeren yada aynı kuralları farklı şekillerde yorumlayan birden çok versiyonu tasarlanabilir ve kullanılabilir.

Policy State: Policy'ler temel olarak Editable, Saved, Published ve Deployed durumlarında bulunabilir. Bir Policy ilk kez oluşturulduğunda zaten otomatik olarak Editable moda geçer. Policy'nin kaydedilmesi sonrası Saved moda atanır. Saved modda düzenlemeler ve testler yapılabilir. Eğer Policy, publish edilirse artık düzenlenemez, değiştirilemez. Yani read-only olarak düşünülebilir. Bu aşama, söz konusu Policy Deploy edilmeden önceki zamandır. Policy, Deploy edildiğindeyse artık versiyonlanmış ve kullanılabilir hale gelmiştir. Ne varki bu moddada üzerinde düzenleme yapılamamaktadır. Dolayısıyla bu aşamadan sonra, Policy içerisinde yazılmış olan kurallarda değişim yapılamaz. Ancak yeni bir versiyonlama veya yeni bir Policy oluşturulması ile sorunlar ortadan kaldırılabilir.

Rules: Çok doğal olarak konunun ana fikri bir takım iş kurallarının uygulanmasıdır. İş kuralları, Policy'ler içerisinde Rule nesneleri ile ifade edilir. Rule'lar kendi içlerinde, koşullandırılacak olan verileri (Fact), bunlarla ilişkili Predication'ları ve aksiyonları (Action) içermektedir.

Facts: Aslında Rule içerisinde yer alan koşullar, karşılaştırmalar ve aksiyonlarda kullanılan veri birimlerini temsil etmektedir. Çok doğal olarak bu nesnenin uygulanan koşul sonrası yapılacak bir takım işlemler ile kuralın bütünü oluşturulmaktadır.

Fact Source: Fact nesnelerinin içeriği, XML ve veritabanı gibi kaynaklardan gelebileceği gibi, sistemin Global Assembly Cache (GAC) alanında yüklü bir assembly içerisindeki.Net tipide olabilir.

Peki bir.Net tipini, BRE içerisinde kullanmak ve herhangibir uygulamada bu tipe ait nesne örneklerini Rules Engine içerisinde tanımlı ilkelere dahil etmek istiyorsak nasıl bir yol izlemeliyiz.

1 - İlk olarak.Net tipini içeren bir Class Library geliştirilir. Bu library içerisinde BRE managed nesnelerini kullanabilmek için varsayılan olarak C:\Program Files\Microsoft BizTalk Server 2006 adresinde yer alan Microsoft.RuleEngine.dll assembly'ının projeye referans edilmesi gerekir.

![blg46_5.gif](/assets/images/2009/blg46_5.gif)

Oluşturduğumuz CompanyRules isimli sınıf kütüphanesinde yer alan kod içeriğimiz ise ilk etapta aşağıdaki gibidir.

```csharp
namespace CompanyRules
{
    public class Product
    {
        public int ProductId { get; set; }
        public int Count { get; set; }        
        public bool StockLevelOk { get; set; }
    }
}
```

Product isimli sınıf içerisinde yer alan Count özelliğinin değerine göre bir takım kurallar tanımlayacağımızı şimdiden söyleyebilirim. Örneğin Count'un belirli bir değerin altında olması halinde StockLevelOk özelliğine false değerinin atanması bir kural olarak düşünülebilir.

2 - Yazılan.Net tipi için mutlaka bir test tipi geliştirilmelidir. Daha önceden de bahsedildiği üzere, Policy, Published veya Deployed modlarına geçildiğinde değiştirilemez. Dolayısıyla test edilebilir olması önemlidir. Nitekim test sonuçlarına bakılarak Fact'lerin tanımlanan Rule'lar için doğru çalışıp çalışmadığı değerlendirilmelidir. Bu amaçla, yine Microsoft.RuleEngine isim alanı altında yer alan IFactCreator arayüzünden (Interface) türeyen bir tip kullanılır. CompanyRules sınıf kütüphanesinde yer alan Product tipi için, IFactCreator arayüzünde türeyen aşağıdaki tip tasarlanmıştır.

```csharp
using System;
using Microsoft.RuleEngine;

namespace CompanyRules
{
    public class ProductFactCreator
        :IFactCreator
    {
        #region IFactCreator Members

        public object[] CreateFacts(RuleSetInfo ruleSetInfo)
        {
            Product prod = new Product
            {
                 Count=100,
                  ProductId=10001                   
            };
            return new object[] { prod };
        }

        public System.Type[] GetFactTypes(RuleSetInfo ruleSetInfo)
        {
            return null;
        }

        #endregion
    }
}
```

Burada yapılan aslında Product biriminin belirtilen bir kural için test edilebilir olmasını sağlamaktır. Bu nedenle, CreateFacts metodu içerisinde örnek bir Product nesne örneği oluşturulmuş ve geriye döndürülmüştür. Aslında burada işleyiş şekli tam anlamıyla ders niteliğindedir. IFactCreator arayüzü, BizTalk tarafında tanımlanmıştır. Bu arayüz, Business Rule Composer programındaki testler için önemlidir. Nitekim dışarıdan bir tipin, var olan Biztalk uygulamasına entegre edilmesini sağlamaktadır. Yani bildiğimiz Plug-In mantığı söz konusudur.

3 - Geliştirilen tiplerin yer aldığı.Net assembly'ının, Business Rule Composer içerisinde kullanılabilmesi için Strong Name Key ile imzalanıp Global Assembly Cache alanına atılmış olması gerekmektedir. Aksi takdirde Business Rule Composer içerisinde kullanılamaz. Tahmin edileceği üzere uygulamamızı Strong Name ile imzalamak için Visual Studio ortamında proje özelliklerinden gerekli ayarlamaları yapabiliriz.

![blg46_7.gif](/assets/images/2009/blg46_7.gif)

Bu işlemin ardındada derlenen assembly, komut satırından GacUtil ile veya basit bir şekilde Windows\Assembly klasörü altına sürükle bırak yöntemi ile install edilir.

![blg46_8.gif](/assets/images/2009/blg46_8.gif)

4 - Business Rule Composer aracından yararlanılarak Policy ve içerisinde yer alan kurallar oluşturulur. Aslında bu adımı çok fazla dert etmemiz gerek yok. Bu konuyu görsel derstede ele alacağımızdan aşağıdak şekilde görülen basit kuralları oluşturmaya çalışsak yeterli olacaktır. Tabiki unutulmaması gereken önemli noktalardan biriside, Fact Explorer kısmında, aşağıdaki ekran görüntüsünde olduğu gibi CompanyRules assembly'ının seçilmesi gerekliliğidir ki bu sayede Rule içerisindeki Fact'ler için kullanılacak özellikler ele alınabilecektir.

![blg46_10.gif](/assets/images/2009/blg46_10.gif)

Ve örnek kurallarımız;

Policy2 içerisnde tanımlanan ilk kuralımız Rule1 ismindedir. Bu kurala göre Product nesne örneğinin Count özelliğinin değerinin 5000' in altında olması halinde StockLevelOk özelliğine False değeri atanmaktadır.

![blg46_13.gif](/assets/images/2009/blg46_13.gif)

İkinci kuralımız (Rule 2)' da ise, birinci kuralın zıttı olan durum söz konusudur. Bu kez Count değerinin 5000' den büyük veya eşit olması halinde StockLevelOk özelliğine true değeri atanmaktadır.

![blg46_14.gif](/assets/images/2009/blg46_14.gif)

5 - Kurallar test edilir ve herşey beklendiği gibiyse, dağıtım (Deployement) aşamasına geçilir. Test için yapılması gereken ilk adım, tanımlanan Rule'lar üzerinde testi gerçekleştirecek olan.Net tipinin seçilmesidir. Buda Test düğmesine basıldığında bize sorulmaktadır ki yine GAC içerisinde duran assembly kütüphanemiz zaten söz konusu IFactCreator türevini içermektedir.

![blg46_17.gif](/assets/images/2009/blg46_17.gif)

6 - Test edilen ve test sonuçları beklediğimiz gibi çıkan Policy sırasıyla Publish ve Deploy işlemlerinden geçirilerek kullanıma hazır hale getirilir.

![blg46_15.gif](/assets/images/2009/blg46_15.gif)

7 - Deploy edilen Policy'lerin ve içerdiği kuralların herhangibir.Net uygulamasında kullanılabilmesi için, söz konusu uyulamaya yine Microsoft.RuleEngine.dll assembly'ının referans edilmesi gerekir. Bu adıma gelinmeden önce, 5nci adımda yaptığımız testlerin sonuçlarının doğruluğundan emin olunmalıdır.

![blg46_18.gif](/assets/images/2009/blg46_18.gif)

8 - Kural motorunu kullanacak olan.Net uygulamasında, Microsoft.RuleEngine isim alanı altında yer alan Policy tipinden yararlanılarak, Fact nesnesinin BRE içerisine atılması sağlanır. İşte Company isimli Console uygulamamızda yer alan kodlarımız.

```csharp
using System;
using CompanyRules;
using Microsoft.RuleEngine;

namespace Company
{
    class Program
    {
        static void Main(string[] args)
        {
            Product prd = new Product { Count = 4999, ProductId = 1001 };

            Policy policy = new Policy("Policy2", 1, 0);
            policy.Execute(prd);

            Console.WriteLine("{0} için , Stock Level Ok ? {1}",prd.ProductId,prd.StockLevelOk);

            prd.Count += 10;
            policy.Execute(prd);
            Console.WriteLine("{0} için , Stock Level Ok ? {1}", prd.ProductId, prd.StockLevelOk);

        }
    }
}
```

Görüldüğü üzere ilk olarak Product nesnesi örneklenmektedir. Sonrasında bir Policy nesnesi örneklenir. Burada önemli olan bir noktada Major ve Minor versiyon numalarınında belirtilmesidir. Bu bize şöyle bir avantaj sağlayabilir; bir Policy'nin birden fazla versiyonu olması halinde, program içerisinde hangisinin kullanılacağını seçmemize olanak tanır. Hatta söz konusu değerleri (Policy adı, Major ve Minor numaraları) uygulamaya ait konfigurasyon dosyasından çekilebilir. Böylece kodun içerisinde kesinlikle girilmeden, Policy versiyonlaması ve hangi ilkelerin kullanılacağına karar verilmeside sağlanmış olur. (Konfigurasyon kullanımını bu konunun görsel anlatımında gösteriyor olacağım) Uygulamamızı çalıştırdığımızda aşağıdaki sonuçlar ile karşılaşırız.

![blg46_19.gif](/assets/images/2009/blg46_19.gif)

Böylece geldik zevkli bir konunun daha sonuna. Umarım sizler içinde yararlı olmuştur. Geliştirdiğimiz örnek Biztalk Server 2006' ya ait Business Rule Engine'i kullanmaktadır. Ancak bildiğiniz üzere bir süre öncede Biztalk Server 2009 ürünü yayınlanmıştır. Dolayısıyla bu konu ile ilişkili araştırmalarımı 2009 sürümü üzerinden devam ettiriyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloBRE.rar (37,09 kb)](/assets/files/2009/HelloBRE.rar)

[Meraklısı için Business Rule Engine kavramı ile ilişkili detaylı bilgi](http://en.wikipedia.org/wiki/Business_rules_engine)