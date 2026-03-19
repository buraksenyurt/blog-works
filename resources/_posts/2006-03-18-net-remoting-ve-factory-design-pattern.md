---
layout: post
title: ".Net Remoting ve Factory Design Pattern"
date: 2006-03-18 12:00:00 +0300
description: "Factory Design Pattern (Fabrika Tasarım Deseni), istemcilerin ihtiyaç duyduğu nesneleri oluşturmak için özel bir nesnenin kullanıldığı mimariyi ele alır. Öyleki bu tasarım deseninde istemcinin, kullanacağı asıl nesnenin nasıl üretileceği hakkında herhangibir bilgiye sahibi olması gerekmez. Bu örnekleme işini üstlenen fabrikanın (Factory) kendisidir. Biz bu makalemizde, Factory Design Pattern' in .Net Remoting içerisinde kullanılışını incelemeye çalışacağız. Makaleyi kolay takip edebilmeniz açısından Remoting ile ilgili temel bilgilere aşina olmanız önemlidir. Factory tasarım deseninin 3 önemli parçası vardır. Client, Factory ve Product."
categories:
  - dotnet-remoting
tags:
  - dotnet-remoting
  - csharp
  - dotnet
---
# .Net Remoting ve Factory Design Pattern
Değerli Okurlarım Merhabalar,

Factory Design Pattern (Fabrika Tasarım Deseni), istemcilerin ihtiyaç duyduğu nesneleri oluşturmak için özel bir nesnenin kullanıldığı mimariyi ele alır. Öyleki bu tasarım deseninde istemcinin, kullanacağı asıl nesnenin nasıl üretileceği hakkında herhangibir bilgiye sahibi olması gerekmez. Bu örnekleme işini üstlenen fabrikanın (Factory) kendisidir. Biz bu makalemizde, Factory Design Pattern'in.Net Remoting içerisinde kullanılışını incelemeye çalışacağız. Makaleyi kolay takip edebilmeniz açısından Remoting ile ilgili temel bilgilere aşina olmanız önemlidir. Factory tasarım deseninin 3 önemli parçası vardır. Client, Factory ve Product.

![mk152_1.gif](/assets/images/2006/mk152_1.gif)

İstemcilerin (Clients) amacı Product tipinden nesne örneklerini kullanmaktır. Bunun için mutlaka ve mutlaka Product tipinden nesne örneklerinin oluşturumasına ihtiyaç vardır. İstemci bu üretim işlemini doğrudan değil, Factory nesne örnekleri üzerinden yapar. Dolayısıyla her hangibir Product tipinin üretilmesi aşamasında oluşabilecek değişikliklerden sorumlu olan tip Factory nesnesi olacaktır. Bu bir anlamda istemcilerin, ilgilendikleri Product nesnesinin nasıl örneklendirildiklerini bilmemesi anlamına gelir. Bunu soyutlama (abstraction) olarakta tanımlayabiliriz. Bu sebepten dolayı Factory tasarım deseni özellikle remoting uygulamalarında sıkça kullanılmaktadır. Çünkü uzak nesnelerin (ki burada product nesnelerimiz olacak) oluşturulması (Creation) kısmında zaman içerisinde güncellemeler ve değişiklikler olabilir. İşte bu değişiklikleri ele alacak ve bu sorumluluğu taşıyacak olan kısım istemci değil Factory ' nin kendisi olacaktır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Factory tasarım deseni, nesnelerin oluşturulması ile ilgilendiğinden constructor metodları aktif şekilde kullanır. Bu nedenlerden dolayı Creational Pattern'ler kategorisinde yer almaktadır.

Şimdi dilerseniz konuyu daha fazla karmaşıklaştırmadan teoriden uzaklaşalım ve factory tasarım desenini ele alabileceğimiz bir örnek geliştirelim. Bu örneğimizde istemciler, ürün olarak kullanacakları nesne örnekleri üzerinden uzak sunucuda yer alan bir veritabanına bağlanacaklar ve kendileri için gerekli bilgileri tedarik edecekler. Elbetteki, ürünlerin oluşturulması işlevini tamamen Factory nesnemize devredeceğiz. İlk olarak izleyeceğimiz yoldan bahsedelim.

Geliştirme Adımlarımız

1 -
Factory ve Product nesneleri için gerekli abstract sınıflarımızı public bir class library içerisinde oluştururuz.

2 -
Server tarafı için gerekli abstract sınıflarımızdan türetilen Factory ve Product sınıflarımızı yazarız.

3 -
Server tarafında gerekli remoting kodlarını yazarız.

4 -
İstemci tarafında gerekli remoting kodlarını yazarız.

İlk olarak abstract sınıflarımızı yazmamız gerekiyor. Bunları bir class library içerisinde tutmamızın en önemli nedeni, hem server tarafında hemde istemci tarafında ihtiyacımızın olması ve referans olarak eklememiz gerektiği. Bu nedenle önce bir class library projesi açalım ve aşağıdaki abstract sınıflarımızı oluşturalım.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Abstract sınıflar, normal sınıflar gibi üyeler içerebilen, örneklendirilemeyen (instance create edilemez) ve kendisinden türeyen tiplerin mutlaka override etmesi gereken metod, özellik gibi üyelerin tanımlamalarını içeren (kod bloksuz halleri) tiplerdir.

![mk152_2.gif](/assets/images/2006/mk152_2.gif)

```csharp
namespace FactoryProductBase
{
    public abstract class ProductBase : MarshalByRefObject
    {
        public abstract string GetContactInfo();
    }

    public abstract class FactoryBase : MarshalByRefObject
    {
        public abstract ProductBase CreateProduct(int ContactID);
    }
}
```

Şimdi burada yaptıklarımızdan kısaca bahsedelim. Öncelikli olarak ProductBase abstract sınıfımız yazdık. Bu sınıfımız herhangibir istemcinin asıl olarak çalışmak isteyeceği nesnelerin uygulayacağı bir metod bildirimi içermekte. Amacımız bu abstract sınıfı uygulayan bir Product tipi ile AdventureWorks veritabanında yer alan Contact tablsoundan belirli bir ContactID'ye ait bilgileri string formatta alabilmek.

Üretimden sorumlu olan FactoryBase isimli abstract sınıfımız ise, kendisini uygulayacak olan herhangibir Factory tipinin, Product üretme işini nasıl yapması gerektiğini bildiren bir abstract metod içeriyor. Dikkat ederseniz bu abstract metodumuzun dönüş tipi ProductBase'dir. Buda FactoryBase'den türemiş bir sınıfın override edilecek olan CreateProduct isimli metodunun, ProductBase tipinden türetilmiş olan bir sınıfa ait nesne örneğini döndürebileceği anlamına gelmektedir. Bu ilişkiyi sunucu tarafımızda çalışacak olan asıl sınıflarımızı yazarken kullanacağız.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Her iki abstract sınıfında, remoting içerisinde kullanılabilmesini sağlamak için MarshalByRefObject'dan türetildiğine dikkat edin. MarshalByRefObject bildiğiniz gibi, istemcinin remote nesnenin referansı ile çalışabilmesini sağlar.

Abstract sınıflarımızı hazırladıktan sonra sıra geldi sunucu tarafını programlamaya. Sunucu tarafında, FactoryBase ve ProductBase sınıflarından türeyen tiplerimizi yazarak işe başlayacağız. Ama öncesinde, sunucu uygulamamızı bir console application olarak açalım. Daha sonra ise, System.Runtime.Remoting ve abstract sınıflarımızı barındırdan Class Library'imizi bu uygulamaya referans olarak ekleyelim.

![mk152_3.gif](/assets/images/2006/mk152_3.gif)

Şimdi Factory ve Product sınıflarımızı yazmamız gerekiyor.

![mk152_4.gif](/assets/images/2006/mk152_4.gif)

```csharp
public class Product : ProductBase
{
    string _ContactInfo;

    public Product(int ContactID)
    {
        _ContactInfo = "";
        using (SqlConnection con = new SqlConnection("data source=MANCHESTER;database=AdventureWorks;integrated security=SSPI"))
        {
            using(SqlCommand cmd=new SqlCommand("Select ContactID,FirstName,MiddleName,LastName From Person.Contact Where     ContactID=@ContactID",con))
            {
                cmd.Parameters.Add("@ContactID", SqlDbType.Int);
                cmd.Parameters["@ContactID"].Value = ContactID;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dr.Read();
                StringBuilder sb = new StringBuilder(_ContactInfo);
                sb.Append(dr["ContactID"] + " ");
                sb.Append(dr["FirstName"] + " ");
                sb.Append(dr["MiddleName"] + " ");
                sb.Append(dr["LastName"] + " ");
                dr.Close();
                _ContactInfo = sb.ToString();
            }
        }
    }

    public override string GetContactInfo()
    {
        return _ContactInfo;
    }
}

public class Factory : FactoryBase
{
    public override ProductBase CreateProduct(int ContactID)
    {
        return new Product(ContactID);
    }
}
```

Kısaca sınıflarımızın çalışma şekline bakalım. Factory sınıfımız FactoryBase abstract sınıfından türemiştir. Bu nedenle CreateProduct isimli metodu override etmek zorundadır. CreateProduct metodumuz dikkat ederseniz, ProductBase sınıfından türettiğimiz Product tipinden bir nesne örneğini geriye döndürmektedir. Burada elbeteki devreye Product sınıfı içerisinde yazdığımız yapıcı metod (constructor) girer.

Bu metod içerisinde kısaca veritabanı bağlantı işlemlerimizi yapıp, Contact tablosundan ContactID, FirstName, MiddleName ve LastName alanlarının değerlerini okuyup stringBuilder yardımıyla sınıf içerisinde yer alan _ContactInfo isimli string'e aktarmaktayız. Son olarak Factory tipimizin override ettiği GetContactInfo isimli metodumuzda _ContactInfo değişkenimizin içeriğini geriye döndürmekte. Sıra geldi, server tarafında remoting için yapmamız gereken işlemlere. Bunun için server tarafımızdaki console uygulamamızın main metodunun kodlarını aşağıdaki gibi değiştirmemiz gerekiyor.

```csharp
static void Main(string[] args)
{
    TcpServerChannel srvChannel = new TcpServerChannel(8000);
    ChannelServices.RegisterChannel(srvChannel,true);
    RemotingConfiguration.RegisterWellKnownServiceType(typeof(Factory), "Server/Factory", WellKnownObjectMode.SingleCall);
    Console.WriteLine("Sunucu dinlemede...");
    Console.ReadLine();
}
```

İlk olarak, Tcp protokolü üzerinden haberleşmeyi tercih ettiğimizden, istemciden gelecek talepleri dinleyecek bir portu (örneğimizde 8000 numaralı port) kullanıma açmamız gerekiyor. Bu amaçla bir TcpServerChannel nesnesi oluşturduk ve bunu siteme register ettik. Sonrasında ise SingelCall tekniğine göre (yani her istemci için server tarafında ayrı birer remote object referansı oluşturulması), istemcilerin server ile iletişim kurabileceği URI bilgisini hazırlıyoruz. Bunun için bir WellKnowServiceTypeEntry nesnesini, RemotingConfiguration.RegisterWellKnowServiceType metodu içerisinde oluşturduk. Dolayısıyla sunucu tarafında artık şunu söylemiş oluyuroz. tcp://MANCHESTER:8000/Server/Factory adresini, istemcilerin kullanımına açıyoruz. Bu URI bilgisine göre istemciler MANCHESTER isimli sunucuya 8000 numaralı porttan bağlanacak ve Server isim alanındaki Factory isimli sınıfa ait nesne örneklerinin referansları ile konuşacak. Sunucu tarafımızıda hazırladıktan sonra sıra geldi istemci tarafındaki kodları yazmaya. Yine server tarafına benzer olarak istemci tarafındada bizim için gerekli olan class library ve System.Runtime.Remoting isim alanlarını açıkça eklememiz gerekiyor.

![mk152_5.gif](/assets/images/2006/mk152_5.gif)

Şimdi istemci tarafımızdaki kodları yazalım.

```csharp
static void Main(string[] args)
{
    TcpClientChannel clientChannel = new TcpClientChannel();
    ChannelServices.RegisterChannel(clientChannel, true);

    FactoryBase fb = (FactoryBase)Activator.GetObject(typeof(FactoryBase), "tcp://MANCHESTER:8000/Server/Factory");
    ProductBase pb = fb.CreateProduct(1);
    Console.WriteLine(pb.GetContactInfo());
}
```

İstemci tarafında, sunucu tarafı ile haberleşebilmemiz için bu kez bir TcpClientChannel nesnesine ihtiyacımız var. Bizim için burada önemli olan, server tarafından bir FactoryBase tipinden nesne örneğine ait referansı almak için Activator sınıfının GetObject static metodunu kullanıyor oluşumuz. Bu metod sayesinde, server tarafından FactoryBase tipinden bir referansı kullanacağımızı söylemiş oluyoruz. Ancak dikkat edin sadece söylüyoruz. Henüz bu referansı sunucudan istemciye almış değiliz. Nitekim sunucu üzerindeki referanslar, istemcide bir metod çağırımı yapıldığı zaman oluşturulmaktadır.

Sonrasında ise, Product tipinden nesne örneğimizin ilgili metodunu çalıştırabilmek için, elde ettiğimiz FactoryBase referansının CretaeProduct isimli metodunu kullandık. Böylece sunucu tarafındaki Product tipine erişip, 1 numaralı Contact bilgisini istemci tarafına taşıyabiliyoruz. Burada önemli olan nokta, Product tipine ait bir referansı istemcinin doğrudan create etmeyişi. Bu işlemi FactoryBase'in CreateProduct isimli metodu bizzat yapmakta. Dolayısıyla Product tipine ait bir referansın nasıl oluşturulduğu, istemci tarafından tamamen soyutlaştırılmış olmaktadır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
İstemci tarafında FactoryBase ve ProductBase abstract sınıflarını kullanmamıza rağmen, sunucu tarafındaki Factory ve Product tiplerinin bazı üyelerini ele alabilmekteyiz. İşte bu polimorfizm'in bir etkisidir. Çünkü abstract tipler, kendilerinden türeyen tiplere ait referansları taşıdıklarında, türemiş tiplerin override edilmiş üyelerine erişebilirler.

Uygulamamızı test etmek için öncelikle server tarafını çalıştırmalı ve sunucuyu dinlemeye hazır hale getirmeliyiz. Eğer sisteminizde yükü bir firewall var ise bu isteğinizi onaylayıp onaylamadığınızı sorabilir. (Örneğin Xp işletim sisteminin Firewall programı devreye gittiğinde Unblock seçeneğini işaretlemeniz lazım.) Burada işlemi onaylayıp portumuzu remote iletişim için açmamız gerekiyor. Sonrasında ise istemci uygulamamızı çalıştırıp sonuçları görebiliriz.

![mk152_6.gif](/assets/images/2006/mk152_6.gif)

Remoting'in uygulandığı bu örneğin başarılı bir şekilde çalışıp çalışmadığını kontrol etmek için, istemci uygulamanızı sunucuyu çalıştırmadan yürütmeyi deneyin. CreateProduct isimli metodun çağırıldığı yerde SocketException tipinden bir istisna alırız. Bunun sebebi, sunucu tarafının çalışmıyor oluşu ve bu sebeple bağlantı sağlanamasıdır.

![mk152_7.gif](/assets/images/2006/mk152_7.gif)

Bu makalemizde, Factory Design Pattern'in, gerçek hayat uyarlamalarından birisi olan.Net Remoting'deki yerini incelemeye çalıştık. Kısaca tekrar etmek gerekirse Factory deseni bir istemcinin, kullanacağı ürünün oluşturulması sorumluluğunu fabrikaya (Factory) devreden bir mimari sunmaktadır. Bu da istemcinin, kullanacağı ürünün nasıl oluşturulacağını bilmek zorunda olmaması anlamına gelir. Dahası, ürün tarafındaki nesnelerin oluşturulması sırasındaki değişiklikler istemciyi etkilemeyecektir. Bu sadece fabrika nesnelerinin bilmesi gereken bir durumdur. Böylece uygulamaların daha kolay genişletilebilmesi sağlanabilir. Tahmin edeceğiniz gibi bir ürünün (Product) ın oluşturulması sırasındaki değişikliklerin sadece sunucu tarafında yapılması yeterli olacaktır. Çünkü istemci tarafında bu tiplere ait referanslar yerine bu tiplerin türediği referansları yer almaktadır. Buda mimariyi en azından ürünlerin üretiliş şeklini tek bir merkezden güdümleyebileceğimiz anlamına gelir. Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulamalı indirmek için tıklayın.](/assets/files/2006/FactoryPattern.rar)