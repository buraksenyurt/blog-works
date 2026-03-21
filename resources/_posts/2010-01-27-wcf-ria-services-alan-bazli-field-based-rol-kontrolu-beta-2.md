---
layout: post
title: "WCF RIA Services - Alan Bazlı(Field Based) Rol Kontrolü [Beta 2]"
date: 2010-01-27 02:33:00 +0300
categories:
  - wcf-eco-system
  - wcf-ria-services
tags:
  - wcf-ria-services
  - .net-ria-services
  - windows-communication-foundation
  - wcf-eco-system
---
Hani bazen insanın aklına son derece zekice fikirler gelir ya...Sene 1992. Lise öğrencisiyim. Bazı akşamlar yazlığımızdaki odamda üniversiteye hazırlanmaya çalışırdım. Güzel yaz gecelerinde, tertemiz ada ikliminde, mis gibi kokan iyotlu deniz suyunun çok yakınlarında konsantre olmak her ne kadar çok zor olsa da, buna mecburdum. Odamdaki flöresan ışığını çalışma ortamı için hiç uygun bulmazdım. Bunun yerine sarı ışığı tercih ederdim ve aynen yandaki şekilde görülene benzer bir gece lambam vardı.

![blg119_Giris.jpg](/assets/images/2010/blg119_Giris.jpg)

Aslında lambanın etrafında şık bir küre bulunmaktaydı fakat sakarlığıyla bilinen bendeniz onu bir ara kırmıştım. Tabi hal böyle olunca şöyle bir sorunla karşılaştım. Işık direkt olarak gözüme geliyor ve çok rahatsız ediyordu. Çözüm olarak nemi yaptım. Dahiyane bir fikirle Amerika'daki bir arkadaşımın hediye ettiği Newyork Nicks takımının logosunu taşıyan şapkayı, lambanın üstüne güzelce yerleştirdim. Özellikle ışığın gözüme direkt olarak girmesini engelleyen ama etrafı ve okuduklarımı görmemi sağlayan bir açıyı düşünüp, ölçüp biçerek, dikkatlice yerleştirdim. Kendimle gurur duyuyordum. Bu zeka ile NASA'ya bile gidebilirim diye düşünüyordum

![Tongue out](/assets/images/2010/smiley-tongue-out.gif)

Ancak gecenin ilerleyen saatlerinde değil NASA, sıradan bir bölümü bile kazanmamın zor olduğuna kanaat getirdim. Nitekim ampülün zaman içerisinde çevreye yaydığı aşırı ısıyı tahmin edememiştim. Ancak odanın içerisine bir yanık kokusu yayıldığında bir şeylerin ters gittiğinin farkına varabilmiştim. En nihayetinde güzelim şapkanın ortasında kocaman bir yanık izi ve erimiş kumaş parçaları ile kala kaldım. Neredeyse koca bir delik açılmıştı.

![Laughing](/assets/images/2010/smiley-laughing.gif)

İşte geçen gün yine böyle dahiyane bir fikir gelir mi aklıma diye düşünürken, WCF RIA Servis operasyonlarından dönen Entity nesnelerinin alanlarını rol bazlı olarak ele alabilir miyiz diye sorgulamaya başladım. Peki neden böyle bir ihtiyacımız olsun? Çok basit bir sebep öne sürebiliriz. Sunucu tarafındaki servisten dönen Entity örnekleri içerisindeki alanlarının bazılarının, Login olan kullanıcı tarafından görülmemesi veya kullanılamaması istenebilir. Malum DomainService tipi içerisinde yer alan operasyonlarda Login olan kullanıcının içerisinde bulunduğu rol elde edilebilmektedir. Buna göre sorgunun üreteceği çıktı içerisine dahil edilecek alanların yetkiye göre oluşturulması sağlanabilir...mi acaba? Durumu örnek bir senaryo üzerinden incelersek çok daha anlaşılır olacaktır. Öncelikli olarak AdventureWorks veritabanı içerisinde yer alan SalesOrderDetail isimli tabloyu kullanmak istediğimizi düşünelim.

![blg119_EntityModelLast.gif](/assets/images/2010/blg119_EntityModelLast.gif)

Örnek senaryomuzda çok doğal olarak Authentication alt yapısınında tesis edilmiş olması gerekmektedir. WCF RIA Service'lerinde Authentication Domain Service kullanımından daha önceki yazılarımızda bol bol bahsettiğimizden bu detayları atlıyoruz. Ancak ASP.NET Membership tabanlı olarak kurulan Authentication alt yapısı içerisinde testler için iki rol olduğunu söyleyebiliriz. AuthorizedSalesPerson ve JuniorSalesPerson. Senaryomuza göre güya AuthorizedSalesPerson rolünden gelen kullanıcılar UnitPriceDiscount alanının değerini görebilirken, JuniorSalesPerson rolündeki kullanıcılar göremeyecektir. Başlangıçta AdventureDomainService isimli Domain Service sınıfını aşağıdaki gibi düzenlediğimizi düşünelim.

```csharp
namespace RoleBasedFields.Web
{
    using System.Linq;
    using System.Web.DomainServices;
    using System.Web.DomainServices.Providers;
    using System.Web.Ria;

    [RequiresAuthentication]
    [EnableClientAccess()]
    public class AdventureDomainService 
        : LinqToEntitiesDomainService<AdventureWorksEntities>
    {
        public IQueryable<SalesOrderDetail> GetSalesOrderDetails()
        {
            return (from sod in ObjectContext.SalesOrderDetails
                    select sod).Take(50);
        }
    }
}
```

> 121317 (Yüz yirmi bir bin üçyüz on yedi)...SalesOrderDetail tablosunda bu kadar satır bulunmaktadır. Bu satırların tamamını istemci tarafına göndermek performans açısında tercih edilmemelidir. Zaten WCF RIA Service'lerin kullanımı ile ilişkili best practices tiyolarında, üretilen standart sorguların filtreler ile düzenlenmesi önerilmektedir. En azından dönen veri içeriğinin bir gözden geçirilerek gerektiğinde performans için filtrelenmesi düşünülmelidir. Bu sebepten örneğimizde Take genişletme metodundan (Extension Method) yararlanılarak ilk 50 satırın alınması sağlanmıştır.

İlk etapta sunucu tarafındaki operasyonda herhangibir rol kontrolü yapılmamaktadır. Buna göre Login olan bir kullanıcı için ekran çıktısı aşağıdaki gibi olacaktır.

![blg119_Runtime1.gif](/assets/images/2010/blg119_Runtime1.gif)

Ancak örnek senaryomuza göre JuniorSalesPerson rolünde olan bill isimli kullanıcının UnitPriceDiscount alanını görmemesi veya anlamlandıramaması gerekmektedir.(Anlamlandıramamasının ne kadar zor olduğunu biraz sonra anlayacağız) Buna göre sunucu tarafında yer alan operasyonun özelleştirilmesi gerekmektedir. Aslında yazımızın ulaşmak istediği tek nokta budur. Peki ama nasıl?

![Wink](/assets/images/2010/smiley-wink.gif)

Sonuçta istemci ve sunucu tarafında eş olan SalesOrderDetail Entity sınıf bilgisini çalışma zamanında değiştirmemiz şu etapta pek mümkün değildir. Akla ilk gelen yöntem result set çekilirken role göre gösterilmesi istenmeyen alana örneğin null değer atanmasını sağlamak olabilir. (Bu konu ile ilişkili olaraktan yaptığım araştırmalarda, blog girdisini hazırladığım tarih itibariyle [Brad Abrams'ın ilgili yazısında](http://blogs.msdn.com/brada/archive/2009/12/08/field-level-access-with-ria-services.aspx)bu tip bir teknik uygulandığını gördüm) Tabi örneğimizdeki alan null değer almamaktadır. Buna göre belki -1 değer atanması sağlanabilir. Ama bu durumdada alan yine görülebilir olacaktır.

![Undecided](/assets/images/2010/smiley-undecided.gif)

Aşağıdaki kod parçasını göz önüne alalım.

```csharp
public IQueryable<SalesOrderDetail> GetSalesOrderDetails()
{
	var resultSet = (from sod in ObjectContext.SalesOrderDetails
					 select sod).Take(50);

	foreach (var result in resultSet)
	{
		if (ServiceContext.User.IsInRole("JuniorSalesPerson"))
			result.UnitPriceDiscount = -1;
	}

	return resultSet;
}
```

Bu kod parçasında görüldüğü gibi resultSet gönderilmeden önce her bir satırı taranmakta ve ServiceContext üzerinden elde edilen güncel kullanıcının rolüne bakılarak UnitPriceDiscount alanına -1 değer atanması sağlanmaktadır. Buna göre çalışma zamanı çıktısı bill isimli kullanıcı için aşağıdaki gibi olacaktır.

![blg119_Runtime2.gif](/assets/images/2010/blg119_Runtime2.gif)

Peki istediğimiz bu muydu?

Kesinlikle değil. Bizim hayalimiz ilgili alanın istemci tarafından görülmemesini sunucudaki operasyon üzerinden sağlamaktı. Oysaki öğrenebildiğimiz sadece şu oldu; servis tarafındaki operasyondon dönen resultSet içeriğindeki veriyi istersek Login olan kullanıcının rolüne göre değiştirebiliriz. İşte şu anda lambaya koyduğumuz şapkanın delindiğini görmekteyiz. Benim NASA hayalleri yine yalan oldu anlayacağınız.

![Sealed](/assets/images/2010/smiley-sealed.gif)

Peki ya çözüm?

En ideal çözüm istemci tarafında kullanıcının rolüne göre ilgili alanının gizlenmesi olarak düşünülebilir. Ancak buda optimal bir çözüm olmayacaktır. Nitekim sunucu tarafında ele alınması gereken güvenlik konulu bir iş mantığını istemeden istemci tarafına taşımak zorunda kalmış oluruz. Tabi en büyük sıkıntılardan birisi şudur. Sunucu tarafında bu rol kontrolünü başarabilsek dahi, istemci tarafında gönderilecek entity örneklerinin dinamik olarak değişebiliyor olması gerekecektir. Oldukça zor bir işlem aslında...

Anlaşılan bu konuda WCF RIA Services tarafında bir eksiklik var. Bende en ideal çözüm için araştırmalarıma devam ediyorum. Bakalım lambanın üzerine koyduğumuz şapkadaki deliği kapatabilecek miyiz? Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
