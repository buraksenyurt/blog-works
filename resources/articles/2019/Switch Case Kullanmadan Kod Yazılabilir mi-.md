---
title: "Switch Case Kullanmadan Kod Yazılabilir mi?"
pubDate: 2019-12-25 10:00:00
categories:
  - C#
tags:
  - C#
  - Redis
  - RabbitMQ
  - Concurrency
  - Generics
  - Dependency Management
---

# Switch Case Kullanmadan Kod Yazılabilir mi?
![image.axd](images/image.axd)

İnsanoğlu yağmurlu bir pazar günü evden çıkıp ne yapacağını bilemezken ne hakla ölümsüzlükten bahseder. Bir yazara ait olan bu cümleyi sevgili Serdar Kuzuloğlu'nun yakın zamanda izlediğim söyleşisinden not almışım. İnsanlığın ömrünü uzatmaya çalışması ile ilgili bir konuya atıfta bulunurken ifade etmişti. Oysa karşımızda duran ekolojik denge ve iklim problemleri, yakın gelecekte (2025 deniyor) dünya nüfusunun 1 milyar 250 milyon kadarının içilebilir su kaynaklarına erişemeyeceğini işaret etmekte. Lakin bundan etkilenmeyecek olan ve asıl ömrünü uzatmak isteyen dünya nüfusunun en zengin %1i, söz konusu kıtlığın yaratacağı sorunlardan ve başka felaketlerden korunmak için kendisine dev sığınaklar inşa ediyor, adalar satın alıyormuş. Gerçekten anlaşılması çok zor ve bir o kadar da karmaşık bir durum değil mi? Bu distopik senaryo bir kenara dursun biz geleceğin iyi şeyler getireceğini ümit ederek gelişmeye devam edelim.

Bazen üzerinde çalıştığımız Legacy sistemlerin biriktirdiği teknik borçlar da aynen bu distopik senaryoda olduğu gibi bizi kara kara düşündürür. Bunun önemli sebeplerinden birisi teknik borçlanma konusundaki bilinçsizliğimizdir. Ancak benim farklı bir teorim var. İtiraf etmeliyim ki programlama dillerini bazen çok yanlış bir biçimde öğreniyoruz. Özellikle nesne yönelimli dillerde bu daha fazla öne çıkıyor.

İlk öğrendiğiniz nesne yönelimli programlama dilini düşünün. Çoğunlukla değişken tanımlamaları ile başlayan, karar yapıları ve döngülerle devam eden bir öğretiyi takip ederiz. Bende genellikle bir programlama dilini tanırken bu yolu tercih ediyorum. Lakin tecrübemiz artıp nesne yönelimli dil bilgimizin yanına tasarım kalıpları ile SOLID (Single Responsibility, Open Closed, Liskov Substitution, Interface Segregation, Dependency Inversion) gibi ilkeleri de ekleyince başka bir dili öğrenirken karar yapılarının üzerinde durmalı mıyız tartışabiliriz.

Öyle ki temiz kodlama ilkeleri bir çok halde switch-case/if-else gibi karar yapılarını kullanmadan kod yazmamız gerektiğini öğütlüyor. Şimdi kısa bir süre için ilgilendiğiniz koda veya projeye bakın. Bir switch-case/if-else bloğu arayın. Şöyle kallavi olanlardan bulursanız harika. Mesela case bloklarının içerisinde bol miktarda if-else ifadeleri de olsun. Tam bir kararsızlıklar abidesinin ekranın üst kısmından aşağıya doğru ilerlediğini ama tüm bu hengameyi içinde barındıran metodun gerçekten işe yarar bir şeyler yaptığını farz edin. Şimdi derin bir nefes alın ve o kod parçasını switch-case olmadan nasıl yazabileceğinizi bulmaya çalışın.

Bu noktaya nasıl mı geldim? Pek tabii bir süredir hayatımızda önemli bir yere sahip olan statik kod analiz aracı Sonarqube sayesinde. Son günlerde Cognitive Complexity (Kavramsal Karmaşıklık desek yeridir) değeri ne yazık ki tavan yapmış sınıflar ve üyeleri arasında gezinmekteyiz. Kullandığımız Sonarqube metriklerine göre bir metodun Cognitive Complexity değerinin 15 puanı aşmaması bekleniyor ancak 200lü değerleri geçen fonksiyonlar var. Complexity değerinin yüksek olması kodun okunurluğu, anlaşılması, yönetimi, bakımı ve test edilebilirliği noktasında çok zayıf olduğu olduğu anlamına gelir. Bu değeri arttıran bir çok bulgu var. Meşhur olanlarından bir tanesi de fazla sayıda switch-case ifadesinin kullanılması. Aslında Sonarqube'a ait aşağıdaki ekran görüntüsü konu hakkında biraz daha fazla fikir verebilir.

> Kavramsal karmaşıklık değeri, temel programlama yapılarının kod içerisindeki kullanımları için belirlenmiş ağırlık değerleri baz alınarak hesaplanmaktadır. Söz gelimi if-then-else için 2, for döngüsü için 3, eş zamanlı çalışan paralel kod parçaları için 4 ağırlık puanı ele alınır. Bu değerlere metodlara aktarılan ve çıkan parametre sayıları gibi kriterler de eklenerek fonksiyonun karmaşıklığı hakkında sayısal bir bilgi elde edilir. Cognitive Complexity puanlamasına etki eden kriterler için Sonarqube'un [şu adresinden](https://www.buraksenyurt.com/admin/app/editor/Complexity değerinin yüksek olması kodun yönetimi, bakımı ve test edilebilirliğinin zor olduğu anlamına gelmektedir.) yararlanabilirsiniz ancak olay sanıldığı kadar basit değil. İşin içerisinde Matematik formüller de var;) [IEEE'nin 2018 basımı şu dokümanında](https://ieeexplore.ieee.org/document/8253447) çok daha fazlasını bulabilirsiniz.

![image.axd](images/image.axd)

Buradaki en önemli sorun metodun bulunduğu sınıfın SOLID ilkelerindeki Open-Closed prensibini ihlal etmesi. Bu ilkeye göre bir nesnenin genişletilmeye açık değiştirilmeye kapalı olması istenir. "Bunu bir örnekle anlamaya çalışsak nasıl olur?" diyorum içimden:) Öyleyse gelin aşağıdaki kod parçasını mercek altına alalım.

```csharp
using System;
using System.Collections.Generic;

namespace Sonarqube.Tests
{
    public enum ProviderDirection
    {
        Kafka,
        Rabbit,
        Redis
    }

    class Program
    {
        static void Main()
        {
            #region Klasik kullanım

            RoleProvider.Ping(ProviderDirection.Kafka,"Birr bilmecem var çocuklar...");

            #endregion
        }
    }

    #region Birinci Durum (Complexity değerini yükselten switch-case kullanımı -Temsili)
	
    public static class RoleProvider
    {
        public static void Ping(ProviderDirection target,string message)
        {
            switch (target)
            {
                case ProviderDirection.Kafka:
                    Console.WriteLine("Kafka->{0}",message);
                    break;
                case ProviderDirection.Rabbit:
                    Console.WriteLine("Rabbit MQ->{0}",message);
                    break;
                case ProviderDirection.Redis:
                    Console.WriteLine("Redis->{0}",message);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion
}
```

Olayı basit bir şekilde analiz etmek için az sayıda case ifadesi kurguladık. Aslında tertemiz bir kod parçamız var. RoleProvider sınıfı içerisindeki Ping metodu bir enum sabitini kullanarak farklı sistemler üzerinden mesaj gönderme işlemini temsil ediyor. Main metodu içerisinde yaptığımız çağrıda nasıl davranması gerektiğini belirliyoruz. Sorun RoleProvider sınıfı içerisine yeni bir provider durumu eklemek istediğimizde ortaya çıkıyor. Bunun için RoleProvider sınıfının kodunu değiştirmek zorundayız. Yani yeni durumu da eklememiz gerekmekte. Eğer RoleProvider sınıfını devasa bir uygulamanın ortak kütüphanelerince kullanılan bir tipi olarak düşünürsek işimiz daha da zorlaşıyor. İşte hem bu ilkenin ihlalini engellemek hem de Sonarqube aracını memnun etmek için başvurabileceğimiz güzel bir yol var. O da strateji tasarım kalıbının bir uyarlaması. Şimdi yukarıdaki kod parçasını aşağıdaki hale getirelim.

![image.axd](images/image.axd)

```csharp
using System;
using System.Collections.Generic;

namespace Sonarqube.Tests
{
    public enum ProviderDirection
    {
        Kafka,
        Rabbit,
        Redis
    }

    class Program
    {
        static void Main()
        {
            #region Strategy Pattern uygulanan çözüm

            ProviderContext strategyContext = new ProviderContext();
            string message = "Acaba nedir nedir? Bisküvi denince akla...";
            strategyContext.Ping(ProviderDirection.Kafka,message);

            #endregion
        }
    }

    #region Strateji kalıbının uygulandığı çözüm
    interface IProviderStrategy
    {
        void Send(string message);
    }

    class KafkaProvider
        : IProviderStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine("Mesaj Kafka'ya gönderilir. {0}", message);
        }
    }

    class RabbitMqProvider
        : IProviderStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine("Mesaj RabbitMQ'ya gönderilir. {0}", message);
        }
    }

    class RedisProvider
        : IProviderStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine("Mesaj Redis'e gönderilir. {0}",message);
        }
    }

    class ProviderContext
    {
        private static Dictionary<ProviderDirection, IProviderStrategy> _providers = new Dictionary<ProviderDirection, IProviderStrategy>();

        //// Belki basit bir Injection kurgusu ile provider'lar içeri alınabilir
        //public void AddProvider(ProviderDirection direction, IProviderStrategy provider)
        //{
        //    _providers.Add(direction, provider);
        //}
        static ProviderContext()
        {
            _providers.Add(ProviderDirection.Kafka, new KafkaProvider());
            _providers.Add(ProviderDirection.Rabbit, new RabbitMqProvider());
            _providers.Add(ProviderDirection.Redis, new RedisProvider());
        }
        public void Ping(ProviderDirection direction,string message)
        {
            _providers[direction].Send(message);
        }
    }

    #endregion
}
```

Evet evet biliyorum. Üç tanecik case bloğu için tonlarca kod yazdık ama duruma böyle bakmamak gerekiyor değil mi?;) Strateji kalıbının bu kullanımı sayesinde kodun daha okunabilir hale geldiğini söyleyebiliriz. Özellikle aşağıya doğru uzayıp giden case bazlı kod bloklarını çevirdiğinizde farkı çok daha net anlayacaksınız. Şimdi kod tarafında neler yaptığımızı kısaca inceleyelim. İstemci (Client) olarak niteleyebileceğimiz program sınıfı belli bir Provider tipine göre mesaj göndermek istiyor. Bunu yaparken enum sabitinden yararlanıyor. Enum sabitinin her elemanı farklı bir davranış biçiminin sergilenmesi anlamına da gelmekte. Eğer Kafka seçiliyse mesaj gönderme ona göre yapılmalı. RabbitMQ seçildiyse de ona göre. İşte bu hal değişikliğinin uyarlanması bizi ister istemez davranışsal tasarım kalıplarına (Behavioral Design Patterns) götürüyor. Bu tip switch-case kullanımlarının önüne geçilmesinde yukarıdaki kod parçasında yer verilen strateji tasarım kalıbının uyarlanması yeterli.

Uyarlamada dikkat edileceği üzere case durumuna giren asıl tipler (KafkaProvider, RabbitMqProvider, RedisProvider) ile uyguladıkları bir interface (IProviderStrategy) söz konusu. Context tipi olarak ProviderContext sınıfı kullanılıyor. Bu sınıf dikkat edileceği üzere enum sabiti ile karşılığı olan provider tiplerinin eşleştirildiği bir koleksiyon barındırmakta. Dolayısıyla sisteme yeni bir case bloğu eklemek demek aslında yeni bir IProviderStrategy türevini tasarlayıp buraya koymak demek. Tabii burada kafa karıştıracak bir durum var. switch-case yapısından kurtulduk ancak Context tipi halen open-closed ilkesini ihlal ediyor gibi. Bir başka deyişle generic Dictionary koleksiyonuna eklenecek bağımlılıkları içeriye enjekte etmeyi de düşünebiliriz. Yorum satırı haline getirilen AddProvider metodunda bunu bir nebze olsun ifade etmeye çalıştım ancak çözümü çok daha şık hale getirebiliriz. Bunu bir düşünün ve nasıl yapacağınıza karar verin.

Ah sonlandırmadan çalışma zamanına ait bir ekran görüntüsü de paylaşırsam çok iyi olacak.

![image.axd](images/image.axd)

Tahminlerime göre aklınıza gelen bir başka soru daha var. "Peki ya if-else kullandığımız senaryolar varsa...Hah haaa" Elbette Cognitive Complexity değerini yükselten durumlardan birisi de if bloklarının çokluğu. Hatta ternary operatörü kullansak bile Sonarqube bunu yutmayabilir:) Strateji kalıbını if-else yapıları için de kurgulayabiliriz elbette ama farklı yaklaşımlar da söz konusu. Bunlardan birisi Chain of Responsbility kalıbının uygulanmasıdır. Buna benzer bir kalıp olup normalde GOF (Gangs of Four) listesinde yer almayan ancak [Steve Smith'in bir Pluralsight eğitimi](https://app.pluralsight.com/library/courses/patterns-library/table-of-contents)nde anlattığı ve tesafüden de olsa çok geç bulduğum Rules tasarım kalıbı da var. Bir başka yazıda farklı ilkeler ile bu kez if-else karar yapısını daha doğru kurgulayarak nasıl ilerleyebileceğimizi incelemeye çalışacağım. Şimdilik bana müsade. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Bakınız: switch-case/if-else gibi yapıları kullanmadan geliştirme yaparken tek seçeneğimiz Strategy veya Rules deseni midir? Örneğin Command Pattern kullanılabilir mi?;)
