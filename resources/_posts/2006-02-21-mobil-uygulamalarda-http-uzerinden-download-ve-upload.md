---
layout: post
title: "Mobil Uygulamalarda Http Üzerinden Download ve Upload"
date: 2006-02-21 10:00:00 +0300
categories:
  - windows-mobile
tags:
  - windows-mobile
  - http
  - download
  - upload
  - smart-device
---
Http protolokü yardımıyla internet üzerinden sunuclar ve istemciler arasında veri alış verişinde bulunmak mobil uygulamalarda çok işe yarayan bir tekniktir. Özellikle binary (ikili) formatta veri transferi için, Http ektili ve verimli bir protokol hizmeti sunar. Bu makalemizde mobil uygulamalar ile web sunucuları arasında veri transferi işlemlerini Http protokolüne göre nasıl yapabileceğimizi incelemeye çalışacağız.

Http protokolü gereği uygulamalarda istekte (request) bulunan ve cevap (response) veren tarafların olması söz konusudur. Bir kaynağa yönelik olarak gerekli isteklerde bulunmak ve bu isteklere karşılık gerekli cevapları üretmek için FCL (Framework Class Library) içerisinde yer alan HttpWebRequest ve HttpWebResponse sınıflarını kullanabiliriz. Bu sınıflar sadece Http protokolünü baz alarak çalışmaktadırlar. HttpWebRequest sınıfı yardımıyla bir kaynağa Http protokolü üzerinden istekte bulunabiliriz.

Bu kaynak çoğunlukla uzak sunucuda yer alır. Benzer şekilde HttpWebResponse sınıfı yardımıyla da istekte bulunduğumuz kayanağa yönelik geri dönüşleri bir başka deyişleri cevapları (response) ele alabiliriz. HttpWebRequest sınıfı aslında WebRequest sınıfından, HttpWebResponse sınıfı ise WebResponse sınıfından türemiştir. WebResponse ve WebRequest sınıfları web üzerindeki çeşitli protokollere göre çalışabilecek sınıflar için ortak özellikleri bünyelerinde toplamaktadır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
WebRequest ve WebResponse sınıfları internet ortamında request/response modeline göre çalışacak mimarileri kullanabilmek için vardır. Özellikle Http protokolünü kullanan bir request/response modeli için bu sınıflardan türeyen HttpWebRequest ve HttpWebResponse sınıfları kullanılır.

Biz örnek olarak bir mobil uygulmanın içerisinde yer aldığı bir senaryoyu göz önüne alacağız. Diyelimki mobil cihazımız ile sokakta gezerken resimler çektik, ses ve görüntü kayıtları yaptık. Bunları internet ortamı üzerinden bir web sunucusuna göndermek isteyebiliriz. (Upload işlemi) Benzer olaraktan tersi durumda söz konusu olabilir. Ya da örneğin bir polis olduğumuzu ve kolumuza bağlı mobil cihazdan suç işleyen kişinin fotoğrafını aldığımızı düşünelim. Bu fotoğraf mobil cihaz taşıyan tüm polis, itfaiye, askeri, sağlık ekiplerince tek bir sunucu üzerinde çekilebilecek bir kaynak olabilir. (Download) Farklı senaryolar düşünülebilir. Http modelinin baz alındığı request/response modeli burada bize mobil cihazlar ile sunucular arasında iletişim kurmamızın yollarından sadece birisidir. Dolayısıyla bu senaryolar için farklı tekniklere ve mimarilerede başvurabilirsiniz. Biz Http protokolü yardımıyla bunu nasıl sağlayabileceğimizi inceleyeceğiz.

İlk olarak işe basit bir Smart Device Application uygulaması oluşturmakla başlayalım. Non-Graphical Application tipinde basit bir uygulama geliştireceğiz. Web sunucumuz tarafında ise, IIS altında isimsiz kullanıcıların (anonymous users) read ve write işlemleri yapmasına izin veren bir virtual directory'miz (sanal klasörümüz) olmalıdır. Bu klasörü biz, http üzerinden mobil cihaza göndereceğimiz ve mobil cihazdan alacağımız dosyaları tutacağımız yer olarak kullanacağız. Çünkü istekte bulunacağımız kaynağa http üzerinden erişebilmemiz ve buradan okuma/yazma işlemlerini yapabilmemiz gerekmektedir.

![mk148_1.gif](/assets/images/2006/mk148_1.gif)

Web sunucusu tarafında yaptığımız bu ayarlardan sonra uygulama tarafındaki kodlarımıza geçebiliriz. İlk olarak Download ve Upload işlemleri sırasında izleyeceğimiz prosedürleri kısaca açıklamaya çalışalım. Download işlemi için öncelikle web sunucusundaki ilgili dosyaya bir istekte (request) bulunmamız gerekecektir. Bu işi HttpWebRequest sınıfına ait bir nesne örneği yardımıyla gerçekleştirebiliriz. İstekte bulunduğumuz kaynaktan veri alabilmek için ise, bir başka deyişle web sunucusu üzerinde yer alan dosyayı mobil cihazımıza aktarabileceğimiz bir stream oluşturabilmek içinse HttpWebResponse sınıfına ait nesne örneğinden yararlanacağız. Her iki nesneyi kullanarak sunucuda duran herhangibir formattaki dosyayı bir stream üzerinden mobil cihaza aktarmak için ise Binary seviyede okuma ve yazma işlemi yapacağımız ilgili sınıfları kullanacağız.

Burada çoğunlukla binary (ikili) formatta okuma işlemi yapacağımızdan BinaryWriter ve BinaryReader sınıflarına ait nesne örneklerine ihtiyacımız olacaktır. BinaryWriter sınıfına ait nesne örneğini mobil cihaz üzerindeki kaynağa binary formatta veri yazmak, BinaryReader sınıfına ait nesne örneğimizi ise istekte bulunduğumuz kaynaktan veri okumak için kullanacağız. İlk olarak uygulamamıza System.Net ve System.IO isim alanları ekleyelim. System.Net isim alanından (namespace) ihtiyacımız olan HttpWebRequest, HttpWebResponse, WebRequest ve WebResponse sınıflarını alırken, System.IO isim alanından da FileStream, BinaryWriter ve BinaryReader sınıflarını çağıracağız.

```csharp
using System;
using System.Data;
using System.IO;
using System.Net;

namespace UsingHttp
{
    class Class1
    {
        static void DosyaIndir()
        {
            HttpWebRequest request=(HttpWebRequest)WebRequest.Create("http://169.254.25.129/MobileFiles/Elvis.gif");
            HttpWebResponse response=(HttpWebResponse)request.GetResponse();

            BinaryReader reader=new BinaryReader(response.GetResponseStream());

            FileStream localFile=new FileStream("Elvis.gif",FileMode.Create);
            BinaryWriter writer=new BinaryWriter(localFile);

            try
            {
                while(true)
                {
                    writer.Write(reader.ReadByte());
                }
            }
            catch
            {
            }
            response.Close();
            writer.Close();
        }
    
        static void Main(string[] args)
        {
            DosyaIndir();
        }
    }
}
```

Şimdi uygulama kodlarımızı kısaca inceleyelim. DosyaIndir isimli metodumuzda ilk olarak HttpWebRequest ve HttpWebResponse sınıflarına ait nesne örneklerimizi oluşturuyoruz. HttpWebRequest nesnemizi create etmek için WebRequest sınıfının static Create metodunu kullanıp, cast işlemi yaptığımıza dikkat edelim. Dikkat ederseniz Create static metodu parametre olarak istekte bulunduğumuz dosyanın adresini alıyor.

Buradaki ip adresi aslında bu uygulamada kullandığımız Pocket Pc 2003 Emulator'ümüzün iletişim kuracağı web sunucusunun adresidir ve sisteme yüklü olan Microsoft LoopBack sanal network kartının verdiği bir değerdir. HttpWebResponse sınıfımıza ait nesne örneğimizi oluştururken ise talepte bulunduğumuz HttpWebRequest sınıfına ait nesne örneğinin GetResponse isimli metodunu kullanıyoruz. Bu metod geriye WebResponse sınıfı tipinden bir nesne örneği döndürdüğünden sonucu HttpWebRequest tipine cast etmemiz gerekiyor. Artık web sunucu üzerindeki MobileFiles klasöründe yer alan Elvis.gif isimli resim dosyasına bir istekte bulunup cevap alabilecek şekilde gerekli kodlamaları yapmış olduk.

BinaryReader sınıfına ait nesne örneğimizi, talepte bulunduğumuz kaynaktan ikili formatta okuma yapmak için kullanıyoruz. BinaryReader sınıfına ait nesne örneğimiz parametre olarak bir stream almaktadır. Bu stream ise, response nesnemizin GetResponseStream isimli metodu ile elde edilmektedir. Daha sonra okuyacağımız byte stream'ini mobil cihaz üzerinde kaydedeceğimiz dosya için gerekli işlemleri yapıyoruz. Bir FileStream nesnesi ile önce bu dosyayı mobil cihazın root klasörü altında fiziki olarak oluşturuyor ve ardından bu dosyaya binary formatta yazacak olan BinaryWriter sınıfına ait nesne örneğimizi oluşturuyoruz.

Yazma işlemi için try,catch bloğu içerisinde yer alan basit bir sonsuz döngü kullanıyoruz. Bu döngü içerisinde response nesnesinin açtığı stream üzerinden okuma işlemi yapan reader nesnesini kullanarak, mobil cihaz üzerindeki fiziki dosyaya yazma işlemini BinaryWriter sınıfının Write metodunu kullanarak gerçekleştiriyoruz. Şimdi uygulamamızı çalıştırırsak web sunucumuzda yer alan Elvis.gif isimli dosyanın mobil cihaza indirilmiş olduğunu görürüz. (Uygulamamızı Pocket PC 2003 Emulator'ü üzerinde test etmekteyiz.)

![mk148_2.gif](/assets/images/2006/mk148_2.gif)

![mk148_3.gif](/assets/images/2006/mk148_3.gif)

Sıra geldi mobil cihazımız üzerinden web sunucusuna binary formatta dosya gönderme işlemine. Uygulamamızda örnek olarak DjBurak isimli gif formatında bir dosya olduğunu göz önüne alacağız. Bu dosya fiziki olarak mobil cihazımız üzerinde projemizi deploy ettiğimiz ilgili klasörde yer alacak.

![mk148_4.gif](/assets/images/2006/mk148_4.gif)

Gelelim Upload işlevinin ana hatlarına. Bu kez HttpWebRequest sınıfı başrolü üstelenecek. Nitekim dosya indirme işleminin tam tersini yapacağız. Yani ilk olarak web sunucusu üzerinde yaratmak istediğimiz dosya için bir HttpWebRequest nesne örneği oluşturacağız. Önemli olan nokta bu nesne için uygulanacak olan veri aktarım metodunun tipi. Biz upload işlemi yaptığımızdan aslında PUT metodunu gerçekleştirmiş oluyoruz. Bundan sonrasında ise her zaman olduğu gibi veri gönderme işlemi için bir stream'e ihtiyacımız olacak. Bu kez bu stream'in yönü mobil cihazımızdan sunucuya doğrudur. Stream üzerinden verileri göndermek için ise, byte tipinde belirli boyuttaki dizileri kullanıp bir buffer alan oluşturacağız. Bu buffer'ı daha sonra ilgili stream üzerinden, web sunucusuna doğru aktaracağız. Buna göre DosyaGonder isimli metodumuzu aşağıdaki gibi yazabiliriz.

```csharp
static void DosyaGonder()
{
    HttpWebRequest request=(HttpWebRequest)WebRequest.Create("http://169.254.25.129/MobileFiles/DjBurak.gif");
    request.Method="PUT";
    request.AllowWriteStreamBuffering=true;
    Stream str=request.GetRequestStream();
    FileStream reader=new FileStream("\\Program Files\\UsingHttp\\DjBurak.gif",FileMode.Open);
    byte[] byteArr=new byte[1024];
    int readLength=reader.Read(byteArr,0,byteArr.Length);
    while(readLength>0)
    {
        str.Write(byteArr,0,readLength);
        readLength=reader.Read(byteArr,0,byteArr.Length);
    }
    reader.Close();
    str.Close();
    request.GetResponse();
}
```

İlk olarak bir HttpWebRequest sınıfına ait bir nesne örneğini oluşturuyoruz. Bu kez WebRequest sınıfımızın static Create metodu, mobil cihazdaki dosyanın, web sunucusu üzerinde hangi isimle yazılacağını belirtecek şekilde bir parametre alıyor. Buna göre Upload işlemi başarılı bir şekilde gerçekleştirildiğinde mobil cihazımızdaki dosyamız web sunucusundaki MobileFiles isimli klasöre DjBurak.gif isimli gif formatındaki resim dosyası olarak kaydedilecektir. Daha sonra request nesnesimiz için Http modelini belirtiyoruz.

PUT, request nesnesinin uzak sunucuya veri göndereceğini belirtmiş oluyor. Verileri 1024 byte'lık diziler halinde göndermeyi planladığımızdan kodlarımızı buna göre tasarlıyoruz. Oluşturduğumuz Stream nesnesi, request nesnesinin gösterdiği kaynağa doğru veri aktarma işini üstlenecek. Veri aktarımından yararlanıp web sunucus üzerindeki dosyamızı oluşturmak için ise her zaman olduğu gibi Stream sınıfımızın Write metodunu kullanıyoruz. Burada byte dizisinin boyutunu arttırarak buffer haline getirililen ve http üzerinden gönderilen paketlerin büyüklüğünü ayarlayabilirisiniz. Uygulamamızı çalıştırdığımızda mobil cihazımız üzerinde yer alan DjBurak.gif isimli dosyasının web sunucusundaki MobileFiles isimli klasör altında oluşturulduğunu görebiliriz.

![mk148_5.gif](/assets/images/2006/mk148_5.gif)

Bu makalemizde basit olarak mobil cihazlarda Http protokolünü kullanarak uzak makineler üzerindeki kaynaklara erişimin nasıl yapılabileceğini incelemeye çalıştık. Binary (ikili) formatta, request/response modeline uygun işlemler gerçekleştirebileceğimizi gördük. Örnek olarak gif formatındaki resim dosyalarını ele aldık. Ancak siz binary formatı kullandığınız sürece herhangibir tipteki dosyayıda bu işlem için kullanabilirsiniz. Buna göre görüntülü bir ses dosyasından tutunda şifrelenmiş bir text dosyasını dahi çift yönlü olarak taşıyabilirisiniz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2006/UsingHttp.rar)