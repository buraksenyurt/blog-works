---
layout: post
title: "Serileştirme (Serialization) İçin Püf Noktalar"
date: 2006-02-02 10:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - serialization
  - tips-and-tricks
---
Uygulamalarımızda kullandığımız tipler (types) pek çok amaçla serileştirilirler (Serialization). Framework Class Library içerisinde var olan pek çok tip serileştirilebilir (Serializable) halde tasarlanmıştır. Bizde çoğu zaman kendi yazmış olduğumuz tipleri serileştirme ihtiyacı duyarız. Örnek olarak, XML Web Servislerinde istemci taleplerine gönderilecek olan nesnelerin, network üzerinde taşıyacağımız paketlerin veya bir web uygulamasında yer alan Session nesnelerinin veritabanında saklanması sırasında kullanılan tiplerin serileştirilmesini göz önüne alabiliriz. Hangi türü olursa olsun serileştirmede dikkat edilmesi gereken bazı noktalar vardır. İşte bu günkü makalemizde özellikle Binary ve SOAP formatlı serileştirmelere yönelik püf noktalara değinmeye çalışacağız.

Kendi yazmış olduğumuz bir tipi serileştirmek için tek yapmamız gereken Serializable niteliğini (Attribute) kullanmaktır. Bu zaten bir tipin serileştirilebilmesi için gerekli kuraldır. Serileştirme doğası gereği, tipin içerisinde yer alan alanları (fields) ele alır ve bu alanların değerlerini isimleri ile birlikte herhangibir stream'e yazabilir. Bu durumda, tip içerisinde var olan alanların da serileştirilebilir olmaları gerekir.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Serileştirilen bir tipin var olan alanlarının serileştirilebilir özelliğe sahip olması, başka bir deyişle Serializable niteliğini uygulamış olması gerekir.

Konuyu daha iyi anlayabilmek için örnekler üzerinden gitmeye çalışacağız..Net Framework 2.0 üzerinde bir Console uygulamasını ele alacağız. Örneğin RadyoKanal ve RadyoSahip isimli iki sınıfımızın olduğu bir örneği göz önüne alalım. Bu örneğimizde temel amacımız RadyoKanal sınıfına ait bir nesne örneğini SOAP formatında seriliştirmek ve DeSerialize işlemine tabi tutmak.

![mk146_1.gif](/assets/images/2006/mk146_1.gif)

RadyoKanal sınıfımız;

```csharp
using System;
using System.Runtime.Serialization;

namespace Serilestirme
{
    [Serializable()]
    public class RadyoKanal
    {
        private string _Frekans;
        private string _KanalAdi;
        private string _MuzikTuru;
        private RadyoSahip _Sahip;

        public RadyoKanal(string fre,string ad,string tur,RadyoSahip s)
        {
            _Frekans=fre;
            _KanalAdi=ad;
            _MuzikTuru=tur;
            _Sahip=s;
        }        
      public override string ToString()
      {
        return _Frekans + " " + _KanalAdi + " " + _MuzikTuru + " " + _Sahip.ToString();
      } 
    }
}
```

RadyoSahip sınıfımız;

```csharp
using System;

namespace Serilestirme
{
    public class RadyoSahip
    {
        private string _AdSoyad;
        private System.Int32 _VergiNo;

        public RadyoSahip(string adS,System.Int32 vergiNo)
        {
            _AdSoyad=adS;
            _VergiNo=vergiNo;
        }
        public override string ToString()
        {
            return _AdSoyad;
        }
    }
}
```

RadyoKanal sınıfımız serileştirilebilir bir tiptir. Bu sınıfa ait herhangibir nesne örneğine Binary veya SOAP formatında serileştirme işlemini uyguladığımızda içerdiği tüm alanlarda serileştirilme işlemine tabi tutucaktır. RadyoKanal isimli sınıfımız içerisinde yer alan alanların tipleri şu an için string ve RadyoSahip'tir. String tipi zaten serileştirilebilir bir sınıftır. Oysaki RadyoSahip isimli sınıfımızın şu aşamada serileştirilme özelliği yoktur. Eğer yukarıdaki sınıflarımızı şağıdaki örnek uygulamada olduğu gibi kullanmaya kalkarsak çalışma zamanıda istisna (Exception) alırız.

```csharp
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace Serilestirme
{
    class Program
    {
        private static void Serialize()
        {
            FileStream fs=null;
            try
            {
                RadyoKanal kanal=new RadyoKanal("999.00","BurkiFM","Alternative Rock",new RadyoSahip("Burak Selim Senyurt",10000));
                fs=new FileStream("Test.xml",FileMode.OpenOrCreate);
                SoapFormatter bf=new SoapFormatter(); 
                bf.Serialize(fs,kanal);
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }
            finally
            {
                fs.Close();
            }
        }

        private static void DeSerialize()
        {
            FileStream fs=null;
            try
            {
                fs=new FileStream("Test.xml",FileMode.Open);
                SoapFormatter sf=new SoapFormatter();
                RadyoKanal kanal=(RadyoKanal)sf.Deserialize(fs);
                Console.WriteLine(kanal.ToString());
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }
            finally
            {
                fs.Close();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Serialize();
            DeSerialize();
        }
    }
}
```

![mk146_2.gif](/assets/images/2006/mk146_2.gif)

Uygulamamızda Soap formatında serialize ve deserialize işlemlerini uygulamaktayız. Bu arada, SoapFormatter'ı kullanabilmek için bu sınıfa ait referansı uygulamaya açıkça eklememiz gerektiğini hatırlatmak isterim. Gelelim hata mesajımıza. Hata mesajı RadyoSahip isimli tipin serileştirilebilir bir tip olarak işaretlenmemiş olduğunu ifade etmektedir. Dolayısıyla tek yapmamız gereken, RadyoSahip isimli sınıfımıza da Serializable niteliğini eklemek olacaktır.

```csharp
[Serializable()]
public class RadyoSahip
```

Bu haliyle uygulamamızı çalıştırdığımızda her hangibir problem ile karşılaşmayız. Nesne örneğimizi xml uzantısı ile SOAP formatında serileştirdiğimizden Binary formata göre okunurluğu daha kolaydır. XML dökümanımıza dikkat ederseniz, RadyoKanal ve RadyoSahip tipleri ayrı node'lar halinde ifade edilmiş ve o anki nesne için sahip oldukları alan değerleride bu node'lar içerisinde ayrıştırılmıştır.

![mk146_3.gif](/assets/images/2006/mk146_3.gif)

![mk146_4.gif](/assets/images/2006/mk146_4.gif)

![dikkat.gif](/assets/images/2006/dikkat.gif)
Kendi yazmış olduğumuz bir tipi serileştirilebilir olarak tanımlamassak, bu tipi kullanan başka tiplerinde serileştirilmesini engellemiş oluruz. Ancak bu sonuç, yazdığımız her tipin serileştirilebilir olması gerektiği zorunluluğunu doğurmaz.

Gelelim bir diğer önemli noktaya. Bazen tipimiz içerisinde yer alan filed (alan)' lardan bazılarının serileştirilme işlemine dahil edilmemesini isteriz. Bunun çeşitli sebepleri olabilir. İlk akla gelen, bir nesnenin gizli olan bazı alansal bilgilerinin serileştirilmesinin önüne geçmektir. Örneğin serileştirilen bir paketin network üzerinde dolaştığını düşünecek olursak, gereksiz bilgilerin bu pakette yer almamasını isteyebiliriz.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Serileştirilen bir tip içerisinde, serileştirilme işlemine dahil edilmesini istemediğimiz alanlar için NonSerialized niteliğini kullanırız.

Örneğin, RadyoSahip isimli sınıfımız içerisinde yer alan _VergiNo isimli alanımızın serileştirme işlemine dahil edilmemesini istediğimizi düşünelim. Tek yapmamız gereken NonSerialized niteliğini bu alana uygulamak olacaktır.

```csharp
[NonSerialized()] 
private System.Int32 _VergiNo;
```

Uygulamamızı bu haliyle çalıştırdığımızda _VergiNo alanının serileştirme işlemine dahil edilmediğini görürüz.

![mk146_5.gif](/assets/images/2006/mk146_5.gif)

Eğer serileştirme işlemi üzerinde tam hakimiyet sağlamak istiyorsak ISerializable arayüzüne (interface) başvurmamız gerekecektir. Burada söz konusu olan hakimiyet serileştirme işlemi sırasında hangi alanların, hangi isimler ile ve hangi sırada yazılacağı ve hatta farklı versiyonlara sahip tiplerin söz konusu olması halinde serileştirme işlemlerinin yönetilebilmesidir. RadyoKanal isimli sınıfımıza ISerializable arayüzünü uyguladığımızı düşünelim. Bu arayüz beraberinde sadece tek bir metodun uygulanma zorunluluğunu getirmektedir. Bu metod GetObjectData'dır.

```csharp
using System;
using System.Runtime.Serialization;

namespace Serilestirme
{
    [Serializable()]
    public class RadyoKanal:ISerializable
    {
        // Diğer üyeler

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Kanal Adi", _KanalAdi);
               info.AddValue("Radyo Frekans", _Frekans);
            info.AddValue("Muzik Turu", _MuzikTuru);
            info.AddValue("Kanal Sahibi", _Sahip);
        }

        #endregion
    }
}
```

GetObjectData metodu, sınıfa ait nesne örneğinin serileştirilmesi işlemi sırasında devreye girer. Şu aşamada SerializationInfo tipinde olan info isimli parametre yardımıyla serileştirme içerisinde yer alacak anahtar-değer (key-value) çiftlerini belirlemekteyiz. (Bildiğiniz gibi, bir tipin serileştirilen tüm alanları anahtar-değer çiftleri esasına dayanılarak aktarılırlar.) Buna göre örneğin RadyoKanal isimli sınıfımızın herhangibir nesne örneğinin o an sahip olduğu _Frekans alanının değeri, serileştirilen dosyada Radyo Frekans ismi ile tutulacaktır. Hatta dikkat ederseniz, ilk iki alanın yazılış sırasıda değiştirilmiştir. Öyleki varsayılan serileştirme halinde, tip içerisindeki alanların diziliş sırası dikkate alınmaktadır. Oysaki biz burada bu sıralamayı GetObjectData metodu içerisinde belirleyebileceğimizi görmekteyiz. Ne varki uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki hata mesajını alırız.

![mk146_6.gif](/assets/images/2006/mk146_6.gif)

Aslında serileştirme işleminde her hangibir problem yoktur. Bunu oluşturulan xml dosyasının içeriğine bakarak görebiliriz.

![mk146_7.gif](/assets/images/2006/mk146_7.gif)

Sorun DeSerialize işlemi sırasında meydana gelmektedir. Bir sınıfa ISerializable arayüzünü uyguladığımızda, nesneye ait alanların eşleştirilme işlemlerini her iki yöndede doğru ve tutarlı bir biçimde yapmamız gerekir. Yani sınıfımza deSerialize işlemi uygulandığı takdirde gereken eşleştirme bilgisini de vermemiz gerekir. Bu amaçla genel bir desen kullanılır. Bu desene göre tipe ait bir private constructor (yapıcı metod) aşağıdaki kod parçasında olduğu gibi kullanılmalıdır.

```csharp
private RadyoKanal(SerializationInfo info, StreamingContext ctx)
{
    _Frekans = info.GetString("Radyo Frekans");
    _KanalAdi = info.GetString("Kanal Adi");
    _MuzikTuru = info.GetString("Muzik Turu");
    _Sahip = (RadyoSahip)info.GetValue("Kanal Sahibi", typeof(RadyoSahip));
}
```

Burada dikkat edecek olursanız SerializationInfo tipine ait Get metodlarını kullanarak stream içerisinde gelen anahtar-değer çiftlerinin, RadyoKanal sınıfı içerisinde denk geldiği üyeleri belirlemekteyiz. Bu haliyle uygulamamızı çalıştırdığımızda her hangibir problem ile karşılaşmayız.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Serileştirilebilir bir sınıfa ISerializable arayüzünü (interface) uyguladığımızda, deserialize işlemininde başarılı olması için tipe ait bir private constructor'ın yazılması ve içeride serileştirilmiş üyelere değer atamasının açıkça yapılması gerekmektedir. Bu yapıcı metoda DeSerialize Constructor ismini verebiliriz.

Elbetteki bir sınıfa ISerializable arayüzünü uyguladığımızda, bu sınıftan türeyecek sınıfları yazarkende dikkat etmemiz gereken hususlar vardır. Eğer taban sınıfı (base class) serileştirilebilir olarak tanımlarsak ve ISerializable arayüzünü uygularsak, ilk olarak türeyen sınıfların (derived class) taban sınıf içindeki DeSerialize Constructor metoduna erişebilmelerini sağlamamız gerekir. Bu nedenle temel sınıf içerisindeki DeSerialize Constructor metodunu protected erişim belirleyicisi ile işaretleriz. Bu konuyu daha iyi anlayabilmek için örneğimize RadyoKanal sınıfından türeyen RadyoPersonel isimli yeni bir sınıf ekleyelim.

![mk146_8.gif](/assets/images/2006/mk146_8.gif)

```csharp
[Serializable()]
class RadyoPersonel:RadyoKanal
{
    private int _PersonelNo;
    private string _PersonelAd;

    public RadyoPersonel(int no, string ad)
    {
        _PersonelNo = no;
        _PersonelAd = ad;
    }
}
```

Bu kez uygulamamızda türeyen sınıfımıza ait bir nesne örneğini serileştirmek istediğimizi göz önüne alalım.

```csharp
private static void Serialize()
{
    FileStream fs=null;
    try
    {
        //RadyoKanal kanal=new RadyoKanal("999.00","BurkiFM","Alternative Rock",new RadyoSahip("Burak Selim Senyurt",10000));
        RadyoPersonel rp = new RadyoPersonel(10001, "DJ Burak");
        fs=new FileStream("Test.xml",FileMode.OpenOrCreate);
        SoapFormatter bf=new SoapFormatter(); 
        bf.Serialize(fs,rp);
    }
    catch(Exception err)
    {
        Console.WriteLine(err.Message);
    }
    finally
    {
        fs.Close();
    }
}

private static void DeSerialize()
{
    FileStream fs=null;
    try
    {
        fs=new FileStream("Test.xml",FileMode.Open);
        SoapFormatter sf=new SoapFormatter();
        RadyoPersonel rp=(RadyoPersonel)sf.Deserialize(fs);
        Console.WriteLine(rp.ToString());
    }
    catch(Exception err)
    {
        Console.WriteLine(err.Message);
    }
    finally
    {
        fs.Close();
    }
}
```

Eğer RadyoPersonel sınıfımız bu haldeyken uygulamamızı çalıştırırsak serileştirme işleminin istediğimiz gibi olmadığını, hatta deserialize işlemi sırasındada aşağıdaki hata mesajını aldığımızı görürüz.

![mk146_9.gif](/assets/images/2006/mk146_9.gif)

Bu hata mesajını ele almadan önce Xml dosyamıza bakarsak, taban sınıfa ait üyelerin null değerler ile işaretlendiğini, RadyoPersonel sınıfına ait hiç bir üyenin ise serileştirme işlemine dahil edilmediğini açıkça görebiliriz. Dolayısıyla serileştirme işleminde de süre gelen bir problem söz konusudur.

![mk146_10.gif](/assets/images/2006/mk146_10.gif)

Burada problem şudur. Taban sınıfımız olan RadyoKanal, ISerializable arayüzünü uygulamış ve serileştirme işlemi sırasında GetObjectData metodunu kullanarak üyeleri stream içerisine aktarmıştır. Oysaki, aynı işlemin türeyen sınıf tarafındanda yapılması gerekmektedir.

![dikkat.gif](/assets/images/2006/dikkat.gif)
ISerializable arayüzünü uygulamış serileştirilebilir bir taban sınıftan türetme işlemi yapıldığında, türeyen sınıfa ait nesne örneklerinin sahip olduğu alanların başarılı bir şekilde serileştirilebilmesi için, Temel sınıftaki GetObjectData prensibinin, türeyen sınıf içerisindede açıkça uygulanması gerekir.

Bunu gerçekleştirmek için taban sınıfa sanal bir metod (virtual method) ekleyebilir ve bunun türeyen sınıf içerisinde ezdirilmesini (override) sağlayabiliriz. İlk olarak taban sınıfımıza sanal metodumuzu eklemeli ve bu metodu ISerializable arayüzünün uyguladığı GetObjectData metodu içerisinde çağırmalıyız. Bu amaçla RadyoKanal sınıfımızda aşağıdaki değişiklikleri yapmamız gerekmektedir.

```csharp
protected virtual void WriteData(SerializationInfo info, StreamingContext context)
{
}

public void GetObjectData(SerializationInfo info, StreamingContext context)
{
    info.AddValue("Kanal Adi", _KanalAdi);
    info.AddValue("Radyo Frekans", _Frekans); 
    info.AddValue("Muzik Turu", _MuzikTuru);
    info.AddValue("Kanal Sahibi", _Sahip);
    WriteData(info, context);
}
```

Diğer yandan, türeyen sınıfımız içerisinde bu sanal metodumuzu aşağıdaki kod parçasında olduğu gibi ezmemiz gerekmektedir.

```csharp
protected override void WriteData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
{
    info.AddValue("Personel Numarasi", _PersonelNo);
    info.AddValue("Personel Adi", _PersonelAd);
}
```

Bu haliyle uygulamamızı çalıştırdığımızda yine yukarıdaki istisnayı alırız. Ancak bu kez serileştirme işleminde, türeyen sınıfımıza ait alanlarında aktarıldığını görürüz. Dolayısıyla serileştirme işlemi sırasında süre gelen problemi aşmış durumdayız. Geriye DeSerialize işlemi sırasında oluşan problem kalmaktadır.

![mk146_11.gif](/assets/images/2006/mk146_11.gif)

Tahmin edeceğiniz gibi DeSerialize işlemi sırasındaki problem, türeyen sınıfın kendisine ait bir DeSerialize Constructor metodu olmayışından kaynaklanmaktadır. Bu metod olmadığı için DeSerialize işlemi sırasında, hangi üyeye hangi değerin hangi stream'den aktarılacağı bilinememektedir.

![dikkat.gif](/assets/images/2006/dikkat.gif)
ISerializable arayüzünü uygulamış serileştirilebilir bir taban sınıftan türetme işlemi yapıldığında, türeyen sınıf için DeSerialization işleminin başarılı bir şekilde yapılabilmesi için, türeyen sınıf içerisindede DeSerialize Constructor metodunun uygulanması gerekir.

Bu problemi çözmek için, türeyen sınıfımıza aşağıdaki constructor metodu eklememiz yeterli olacaktır.

```csharp
private RadyoPersonel(SerializationInfo info, StreamingContext context)
{
    _PersonelNo = info.GetInt32("Personel Numarasi");
    _PersonelAd = info.GetString("Personel Adi");
}
```

Uygulamamız bu haliyle başarılı bir şekilde çalışacak ve hem Serialize hemde DeSerialize işlemlerini başarılı bir şekilde gerçekleştirecektir. RadyoKanal ve RadyoPersonel sınıflarının son haline aşağıdaki UML şemasından daha kolay takip edebiliriz.

![mk146_12.gif](/assets/images/2006/mk146_12.gif)

Böylece geldik bir makalemizin daha sonuna. Makalemiz boyunca geliştirdiğimiz örnek uygulamamızın son halini bu linkten [indirebilirsiniz](/assets/files/2006/Serilestirme.rar). Bir sonraki makalemizde görüşünceye dek, hepinize mutlu günler dilerim.