---
layout: post
title: "Interface (Arayüz) Kullanımına Giriş"
date: 2004-01-08 10:00:00 +0300
description: "Bugünkü makalemizde, nesneye dayalı programlamanın önemli kavramlarından birisi olan arayüzleri incelemeye çalışacağız. Öncelikle, arayüz'ün tanımını yapalım. Bir arayüz, başka sınıflar için bir rehberdir. Bu kısa tanımın arkasında, deryalar gibi bir kavram denizi olduğunu söylemekte yarar buluyorum.. Arayüzün ne olduğunu tam olarak anlayabilmek için belkide asıl kullanım amacına bakmamız gerekmektedir."
categories:
  - csharp
tags:
  - csharp
---
# Interface (Arayüz) Kullanımına Giriş
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, nesneye dayalı programlamanın önemli kavramlarından birisi olan arayüzleri incelemeye çalışacağız. Öncelikle, arayüz'ün tanımını yapalım. Bir arayüz, başka sınıflar için bir rehberdir. Bu kısa tanımın arkasında, deryalar gibi bir kavram denizi olduğunu söylemekte yarar buluyorum.. Arayüzün ne olduğunu tam olarak anlayabilmek için belkide asıl kullanım amacına bakmamız gerekmektedir.

C++ programlama dilinde, sınıflar arasında çok kalıtımlılık söz konusu idi. Yani bir sınıf, birden fazla sınıftan türetilebiliyordu kalıtımsal olarak. Ancak bu teknik bir süre sonra kodların dahada karmaşıklaşmasına ve anlaşılabilirliğin azalmasına neden oluyordu. Bu sebeten ötürü değerli Microsoft mimarları, C# dilinde, bir sınıfın sadece tek bir sınıfı kalıtımsal olarak alabileceği kısıtlmasını getirdiler. Çok kalıtımlık görevini ise anlaşılması daha kolay arayüzlere bıraktılar. İşte arayüzleri kullanmamızın en büyük nedenlerinden birisi budur.

Diğer yandan, uygulamalarımızın geleceği açısından da arayüzlerin çok kullanışlı olabileceğini söylememiz gerekiyor. Düşününkü, bir ekip tarafından yazılan ve geliştirilen bir uygulamada görevlisiniz. Kullandığınız nesnelerin, türetildiği sınıflar zaman içerisinde, gelişen yeniliklere adapte olabilmek amacıyla, sayısız yeni metoda, özelliğe vb.. sahip olduklarını farzedin. Bir süre sonra, nesnelerin türetildiği sınıflar içerisinde yer alan kavram kargaşısını, "bu neyi yapıyordu?, kime yapıyordu?, ne için yapıyordu?" gibi soruların ne kadar çok sorulduğunu düşünün. Oysa uygulamanızdaki sınıfların izleyeceği yolu gösteren rehber (ler) olsa fena mı olurdu? İşte size arayüzler. Bir arayüz oluşturun ve bu arayüzü uygulayan sınıfların hangi metodları, özellikleri vb kullanması gerektiğine karar verin. Programın gelişmesimi gerekiyor?. Yeni niteliklere mi ihtiyacın var? İster kullanılan arayüzleri, birbirlerinden kalıtımsal olarak türetin, ister yeni arayüzler tasarlayın. Tek yapacağınız sınıfların hangi arayüzlerini kullanacağını belirtmek olucaktır.

Bu açıklamalar ışığında bir arayüz nasıl tanımlanır ve hangi üyelere sahiptir bundan bahsedelim.Bir arayüz tanımlanması aşağıdaki gibi yapılır. Yazılan kod bloğunun bir arayüz olduğunu Interface anahtar sözcüğü belirtmektedir. Arayüz isminin başında I harfi kullanıldığına dikkat edin. Bu kullanılan sınıfın bir arayüz olduğunu anlamamıza yarayan bir isim kullanma tekniğidir. Bu sayede, sınıfların kalıtımsal olarak aldığı elemanların arayüz olup olamdığını daha kolayca anlayabiliriz.

```csharp
public interface IArayuz
{

}
```

Tanımlama görüldüğü gibi son derece basit. Şimdi arayüzlerin üyelerine bir göz atalım. Arayüzler, sadece aşağıdaki üyelere sahip olabilirler.

Arayüz Üyeleri

özellikler (properties)

metodlar (methods)

olaylar (events)

indeksleyiciler (indexers)

Tablo 1. Arayüzlerin sahip olabileceği üyeler

Diğer yandan, arayüzler içerisinde aşağıdaki üyeler kesinlikle kullanılamazlar.

Arayüzlerde Kullanılamayan Üyeler

yapıcılar (constructors)

yokediciler (destructors)

alanlar (fields)

Tablo 2. Arayüzlerde kullanılamayan üyeler.

Arayüzler Tablo1 deki üyelere sahip olabilirler. Peki bu üyeler nasıl tanımlanır. Herşeyden önce arayüzler ile ilgili en önemli kural onun bir rehber olmasıdır. Yani arayüzler sadece, kendisini rehber alan sınıfların kullanacağı üyeleri tanımlarlar. Herhangibir kod satırı içermezler. Sadece özelliğin, metodun, olayın veya indeksleyicinin tanımı vardır. Onların kolay okunabilir olmalarını sağlayan ve çoklu kalıtım için tercih edilmelerine neden olan sebepte budur. Örneğin;

```csharp
public interface IArayuz
{
    /* double tipte bir özellik tanımı. get ve set anahtar sözcüklerinin herhangibir blok {} içermediğine dikkat edin. */
    double isim
    {
        get;
        set;
    }

    /* Yanlız okunabilir (ReadOnly) string tipte bir özellik tanımı. */

    string soyisim
    {
        get;
    }
    /* integer değer döndüren ve ili integer parametre alan bir metod tanımı. Metod tanımlarındada metodun dönüş tipi, parametreleri, ismi dışında herhangibir kod satırı olmadığına dikkat edin. */

    int topla(int a, int b);
    /* Dönüş değeri olmayan ve herhangibir parametre almayan bir metod tanımı. */

    void yaz();
    /* Bir indeksleyici tanımı */

    string this[int index]
    {
        get;
        set;
    }
}
```

Görüldüğü gibi sadece tanımlamalar mevcut. Herhangibir kod satırı mevcut değil. Bir arayüz tasarlarken uymamız gereken bir takım önemli kurallar vardır. Bu kurallar aşağıdaki tabloda kısaca listelenmiştir.

1
Bir arayüz'ün tüm üyeleri public kabul edilir. Private, Protected gibi belirtiçler kullanamayız. Bunu yaptığımız takdirde örneğin bir elemanı private tanımladığımız takdirde, derleme zamanında aşağıdaki hatayı alırız.
"The modifier 'private'is not valid for this item"

2
Diğer yandan bir metodu public olarakta tanımlayamayız. Çünkü zaten varsayılan olarak bütün üyeler public tanımlanmış kabul edilir. Bir metodu public tanımladığımızda yine derleme zamanında aşağıdaki hatayı alırız.
"The modifier 'public'is not valid for this item"

3
Bir arayüz, bir yapı (struct)'dan veya bir sınıf (class)'tan kalıtımla türetilemez. Ancak, bir arayüzü başka bir arayüzden veya arayüzlerden kalıtımsal olarak türetebiliriz.

4
Arayüz elemanlarını static olarak tanımlayamayız.

5
Arayüzlerin uygulandığı sınıflar, arayüzde tanımlanan bütün üyeleri kullanmak zorundadır.

Tablo 3. Uyulması gereken kurallar.

Şimdi bu kadar laftan sonra konuyu daha iyi anlayabilmek için basit ama açıklayıcı bir örnek geliştirelim. Önce arayüzümüzü tasarlayalım.

```csharp
public interface IArayuz
{
     void EkranaYaz();
     int Yas
     {
          get;
          set;
     }
     string isim
     {
          get;
          set;
     }
}
```

Şimdide bu arayüzü kullanacak sınıfımızı tasarlayalım.

```csharp
public class Kisiler:IArayuz
{
}
```

Şimdi bu anda uygulamayı derlersek, IArayuz'ündeki elemanları sınıfımız içinde kullanmadığımızdan dolayı aşağıdaki derleme zamanı hatalarını alırız.

'Interfaces1.Kisiler'does not implement interface member 'Interfaces1.IArayuz.EkranaYaz ()'

'Interfaces1.Kisiler'does not implement interface member 'Interfaces1.IArayuz.isim'

'Interfaces1.Kisiler'does not implement interface member 'Interfaces1.IArayuz.Yas'

Görüldüğü gibi kullanmadığımız tüm arayüz üyeleri için bir hata mesajı oluştu. Bu noktada şunu tekrar hatırlatmak istiyorum,

Arayüzlerin uygulandığı sınıflar, arayüzde (lerde) tanımlanan tüm üyeleri kullanmak yani kodlamak zorundadır.

Şimdi sınıfımızı düzgün bir şekilde geliştirelim.

```csharp
public class Kisiler:IArayuz /* Sınıfın kullanacağı arayüz burada belirtiliyor.*/
{
     private int y;
     private string i;

     /* Bir sınıfa bir arayüz uygulamamız, bu sınıfa başka üyeler eklememizi engellemez. Burada örneğin sınıfın yapıcı metodlarınıda düzenledik. */
 
     public Kisiler()
     {
          y=18;
          i="Yok";
     }

     /* Dikkat ederseniz özelliğin herşeyi, arayüzdeki ile aynı olmalıdır. Veri tipi, ismi vb... Bu tüm diğer arayüz üyelerinin, sınıf içerisinde uygulanmasında da geçerlidir. */
 

     public Kisiler(string ad,int yas)
     {
          y=yas;
          i=ad;
     }

     public int Yas
     {
          get
          {
               return y;
          }
          set
          {
               y=value;
          }
     }

     public string Isim
     {
          get
          {
               return i;
          }
          set
          {
               i=value;
          }
     }
     public void EkranaYaz()
     {
          Console.WriteLine("Adım:"+i);
          Console.WriteLine("Yaşım:"+y);
     }
}
```

Şimdi oluşturduğumuz bu sınıfı nasıl kullanacağımıza bakalım.

```csharp
class Class1
{
     static void Main(string[] args)
     {
          Kisiler kisi=new Kisiler("Burak",27);
          Console.WriteLine("Yaşım "+kisi.Yas.ToString());
          Console.WriteLine("Adım "+kisi.Isim);
          Console.WriteLine("-----------");
          kisi.EkranaYaz();
     }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk38_1.gif](/assets/images/2004/mk38_1.gif)

Şekil 1. Uygulamanın Çalışması Sonucu.

Geldik bir makalemizin daha sonuna. Bu makalemizde arayüzlere kısa bir giriş yaptık. Bir sonraki makalemizde, bir sınıfa birden fazla arayüzün nasıl uygulanacağını incelemeye çalışacağız. Hepinize mutlu günler dilerim.