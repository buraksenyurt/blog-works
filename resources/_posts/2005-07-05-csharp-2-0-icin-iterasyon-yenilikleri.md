---
layout: post
title: "C# 2.0 İçin İterasyon Yenilikleri"
date: 2005-07-05 12:00:00 +0300
description: "Bazen kendi yazmış olduğumuz tiplerin dizi bazlı elemanları olur. Uygulamalarımızda, bu elemanlar arasında, elamanların sahipi olan nesne örneği üzerinden ileri yönlü iterasyonlar kurmak isteyebiliriz. Foreach döngüleri belirtilen tip için bu iterasyonu sağlayan bir mekanizmaya sahiptir. Lakin kendi geliştirdiğimiz tiplerin sahip oldukları elemanlar üzerinde, bu tarz bir iterasyonu uygulayabilmek için bir numaratöre ve uygulayıcıya ihtiyacımız vardır. Kısacası, tipimizin elemanları arasında nasıl öteleme yapılabileceğini sisteme öğretmemiz gerekmektedir. Bu işlevselliği kazandırmak için IEnumerable ve IEnumerator arayüzlerini birlikte kullanırız."
categories:
  - csharp
tags:
  - csharp
  - generics
  - visual-studio
---
# C# 2.0 İçin İterasyon Yenilikleri
Değerli Okurlarım Merhabalar,

Bazen kendi yazmış olduğumuz tiplerin dizi bazlı elemanları olur. Uygulamalarımızda, bu elemanlar arasında, elamanların sahipi olan nesne örneği üzerinden ileri yönlü iterasyonlar kurmak isteyebiliriz. Foreach döngüleri belirtilen tip için bu iterasyonu sağlayan bir mekanizmaya sahiptir. Lakin kendi geliştirdiğimiz tiplerin sahip oldukları elemanlar üzerinde, bu tarz bir iterasyonu uygulayabilmek için bir numaratöre ve uygulayıcıya ihtiyacımız vardır. Kısacası, tipimizin elemanları arasında nasıl öteleme yapılabileceğini sisteme öğretmemiz gerekmektedir. Bu işlevselliği kazandırmak için IEnumerable ve IEnumerator arayüzlerini birlikte kullanırız.

Uygulamalarımızda klasik olarak kullandığımız bir iterasyon tekniği vardır. Bu tekniğe göre iterasyon içerisinde kullanılacak olan elemanlar için bir numaratör sağlanır. IEnumerable arayüzünün sağladığı GetEnumerator metodu geriye IEnumerator arayüzü tipinden bir nesne örneği döndürmektedir ve bizim için gerekli olan numaratörün kendisidir. IEnumerator ise çoğunlukla dahili bir sınıfa (inner class) uygulanır ve iterasyon için gerekli asıl metodları sağlar. Bu metodlardan MoveNext bir sonraki elemanın olup olmadığını bool tipinden döndüren bir işleve sahip iken, Current metodu o anki elemanı iterasyona devreder. Bu iki arayüzü kullanarak bir sınıf içindeki elemanlar üzerinde iterasyona izin verecek yapıyı kurmak biraz karmaşıktır. Aslında teknik son derece kolaydır sadece uygulanabilirliği ilk zamanlarda biraz kafa karıştırıcıdır. İlk olarak C# 1.1 için bahsetmiş olduğumuz iterasyon tekniğinin nasıl gerçekleştirildiğini inceleyeceğimiz bir örnek geliştirelim.

Örneğimizde her hangi bir işi simgeleyen bir sınıfımız olacak. Bu sınıfımızın modeli basit olarak aşağıdaki kod parçasında olduğu gibidir. IsBilgisi isimli sınıfımız basit olarak bir işin adını ve sorumlusuna ait bilgileri tutacak şekilde tasarlanmıştır. İşe ait bilgileri tutan iki özelliği ve aşırı yüklenmiş (overload) bir yapıcı metodu (constructor) mevcuttur.

```csharp
public class IsBilgisi 
{
    private string m_IsAdi;
    private string m_IsSorumlu;
    public string IsAdi
    {
        get
        {
            return m_IsAdi;
        }
        set
        {
            m_IsAdi=value;
        }
    }
    public string IsSorumlu
    {
        get
        {
            return m_IsSorumlu;
        }
        set
        {
            m_IsSorumlu=value;
        }
    }

    public IsBilgisi(string isAdi,string isSorumlu)
    {
        m_IsAdi=isAdi;
        m_IsSorumlu=isSorumlu;
    }
}
```

Şimdi IsBilgisi sınıfı tipinden 3 elemanı bünyesinde barındıracak bir listeleme sınıfı tasarlayacağız. Amacımız IsBilgisi tipinden dizinin elemanlarına foreach döngüsünü kullanarak IsListesi nesnesi üzerinden erişebilmek. Yani aşağıdaki kodun çalıştırılmasını sağlamak istiyoruz. Burada dikkat ederseniz foreach döngümüz, "liste isimli IsListesi örneğindeki her bir IsBilgisi tipinden nesne örneği için ilerle" gibisinden bir iterasyon gerçekleştirmektedir.

```csharp
IsListesi liste=new IsListesi();
foreach(IsBilgisi i in liste)
{
      Console.WriteLine(i.IsAdi+" "+i.IsSorumlu);
}
```

Normal şartlarda eğer IsListesi isimli sınıfımıza IEnumerable ve IEnumerator arayüzlerini kullandığımız iterasyon tekniğini uygulamazsak bu kod derleme zamanında foreach döngüsünün uygulanamıyacağına dair hata mesajı verecektir. Yani IsListesi sınıfımızın kodunun başlangıçta aşağıdaki gibi olduğunu düşünürsek;

```csharp
public class IsListesi
{
    static IsBilgisi[] Isler=new IsBilgisi[3]; 
    private void ListeOlustur()
    {
        IsBilgisi is1=new IsBilgisi("Birinci iş","Burak");
        IsBilgisi is2=new IsBilgisi("İkinci iş","Jordan");
        IsBilgisi is3=new IsBilgisi("Üçüncü iş","Vader");
        Isler[0]=is1;
        Isler[1]=is2;
        Isler[2]=is3;
    }

    public IsListesi()
    { 
        ListeOlustur();
    }
}
```

derleme zamanında aşağıdaki hata mesajını alırız.

![dikkat.gif](/assets/images/2005/dikkat.gif)
foreach statement cannot operate on variables of type 'UsingIteratorsCSharp1.IsListesi'because 'UsingIteratorsCSharp1.IsListesi'does not contain a definition for 'GetEnumerator', or it is inaccessible

Çünkü foreach döngüsünün IsListesi sınıfı içindeki IsBilgisi nesnelerine nasıl erişeceği ve onlar üzerinde ileri yönlü nasıl hareket edeceği bilinmemektedir. Bu nedenle IsListesi sınıfımız aşağıdaki gibi oluşturulmalıdır. Buradaki amacımız C# 1.1 için iterasyon tekniğini incelemek olduğundan ana fikirde IsBilgisi sınıfı tipinden 3 elemanlı bir dizi kullanılmaktadır.

```csharp
// Iterasyonu sağlayabilmek için IEnumerable arayüzünü uyguluyoruz.
public class IsListesi:IEnumerable
{
    // IsBilgisi tipinden nesne örneklerini taşıyacak dizimiz tanımlanıyor.
    static IsBilgisi[] Isler=new IsBilgisi[3]; 
    // Isler isimli diziyi dolduracak basit bir metod.
    private void ListeOlustur()
    {
        IsBilgisi is1=new IsBilgisi("Birinci iş","Burak");
        IsBilgisi is2=new IsBilgisi("İkinci iş","Jordan");
        IsBilgisi is3=new IsBilgisi("Üçüncü iş","Vader");
        Isler[0]=is1;
        Isler[1]=is2;
        Isler[2]=is3;
    }

    public IsListesi()
    { 
        // IsListesi nesne örneğimiz oluşturulurken Isler isimli dizimizde IsBilgisi tipinden elemanlar ile dolduruluyor.
        ListeOlustur();
    }

    #region IEnumerable Members

    // GetEnumerator metodu IsListesi sınıfımızdaki Isler dizisinin elemanlarında hareket edebilmemiz için gerekli numaratör nesne örneğini geriye döndürüyor
    public IEnumerator GetEnumerator()
    {
        return new Numarator();
    }

    #endregion

    // Isler dizisindeki elemanlarda foreach döngüsünü kullanarak IsListesi sınıfı üzerinden ileri yönlü ve yanlız okunabilir iterasyon yapmamızı sağlayacak inner class' ımızı olusturuyor ve bu sınıfa IEnumerator arayüzünü uyguluyoruz.
    private class Numarator:IEnumerator
    {
        // Dizideki elemanı temsil edecek bir indeks değeri tanımlıyoruz.
        int indeks=-1;
    
        #region IEnumerator Members
    
        public void Reset()
        {
            indeks=-1;
        }
    
        // Güncel IsBilgisi elemanını indeks değerini kullanarak Isler dizisi üzerinden object tipinde geriye döndüren bir özellik. Dikkat ederseniz yanlızca get bloğu var. Bu nedenle foreach döngülerinin sağladığı iterasyon içinde elemanlar üzerinde değişiklik yapamıyoruz.
        public object Current
        {
            get
            {
                return Isler[indeks];
            }
        }

        // İterasyonun devam edip etmemesine karar verebilmek için, şu anki elemandan sonra gelen bir elemanının varlığı tespit edilmelidir. Bunu MoveNext metodu bool tipinden bir değer döndürerek foreach mekanizmasına anlatır. Bizim kontrol etmemiz gereken güncel indeks değerinin Isler isimli dizinin uzunluğundan fazla olup olmadığıdır.
        public bool MoveNext()
        {
            if(++indeks>=Isler.Length)
                return false;
            else
                return true;
        }
    
        #endregion
    }
}
```

Şimdi bu elemanları kullanan basit bir console uygulmasını çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk128_1.gif](/assets/images/2005/mk128_1.gif)

Her ne kadar uygulamamız istediğimiz biçimde çalışsada bizim için bir takım zorluklar vardır. İlki kod yazımının bir desen dahilinde de olsa uzun oluşudur. İkinci zorluk foreach döngüsü içinde kullanılan elemanlar için tip güvenliğinin (type-safety) olmayışıdır. Dikkat ederseniz IEnumerator arayüzünün sağladığı Current isimli metodumuz object tipinden elemanları geriye döndürmektedir. Oysaki biz sadece IsBilgisi nesnesi tipinden elemanları geriye döndürecek bir Current metodunu pekala isteyebiliriz. Hatta bu bize tip güvenliğinide sağlayacaktır.

İşte C# 2.0 hem uzun kod satırlarının önüne geçen hem de tip güvenliğini sağlayan yeni bir yapı getirmiştir. Yapının temelinde yine IEnumerable arayüzü yer almaktadır. Yeni generic tipleri sayesinde, iterasyonun bizim belirttiğimiz tiplere yönelik olarak gerçekleştirilebilecek olmasıda garanti altına alınmaktadır. Bu da aradığımız tip güvenliğini bize sağlar. Yukarıda geliştirmiş olduğumu yapıyı C# 2.0' da aşağıdaki şekilde düzenleriz.

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace UsingIterators
{
    public class IsListesi:IEnumerable<IsBilgisi> 
    {
        static IsBilgisi[] Isler = new IsBilgisi[3];
        private void ListeOlustur()
        {
            IsBilgisi is1 = new IsBilgisi("Birinci iş", "Burak");
            IsBilgisi is2 = new IsBilgisi("İkinci iş", "Jordan");
            IsBilgisi is3 = new IsBilgisi("Üçüncü iş", "Vader");
            Isler[0] = is1;
            Isler[1] = is2;
            Isler[2] = is3;
        }

        public IsListesi()
        { 
            ListeOlustur();
        }

        #region IEnumerable<IsBilgisi> Members

        IEnumerator<IsBilgisi> IEnumerable<IsBilgisi>.GetEnumerator()
        {
            yield return Isler[0];
            yield return Isler[1];
            yield return Isler[2];
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
```

Uygulama kodumuza kısaca bir göz gezdirirsek en büyük yeniliklerden birisinin IEnumerable arayüzünün uygulanışı sırasında ki generic tip tanımlaması olduğunu farkedebiliriz.

```csharp
public class IsListesi:IEnumerable<IsBilgisi>
```

Bu tanımlama ile basitçe, IsListesi nesnesinin uygulayacağı IEnumerable arayüzünün, IsBilgisi tipinden nesne örnekleri için geçerli olacağı belirtilmektedir. Bu tahmin edebileceğiniz gibi, tip güvenliği dediğimiz ihtiyacı karşılamaktadır (type safety).

Diğer önemli bir değişiklik IEnumerator arayüzünü uygulamış olduğumuz her hangi bir dahili sınıfın (inner class) burada yer almayışıdır. Son olarak yield anahtar sözcüğünün kullanımıda en büyük yeniliktir. yield anahtar sözcüğü C# 2.0 ile gelen yeni anahtar sözcüklerden birisidir. IEnumerator tipinden nesne örneğini geri döndüren metodumuz içerisinde kullanılmaktadır.

```csharp
IEnumerator<IsBilgisi> IEnumerable<IsBilgisi>.GetEnumerator()
{
       yield return Isler[0];
       yield return Isler[1];
       yield return Isler[2];
}
```

Dikkat ederseniz Isler isimli dizimizde yer alan her eleman için yield return söz dizimi kullanılmıştır. Aslında aynı işlevi aşağıdaki kod ilede karşılıyabiliriz. Üstelik bu çok daha profesyonel bir yaklaşımdır. Temel olarak anlamamız gereken yield return'ün ilgili dizi içindeki her bir eleman için kullanılıyor olmasıdır.

```csharp
IEnumerator<IsBilgisi> IEnumerable<IsBilgisi>.GetEnumerator()
{
    for (int i = 0; i < Isler.Length; i++)
    {
        yield return Isler[i];
    }
}
```

Kısacası artık yeni iterasyon modelimizde tek yapmamız gereken, foreach döngüsüne dahil olacak elemanların ilgili numarator metodu içinde yield anahtar sözcüğü kullanılarak geri döndürülmesini sağlamaktır. Örneğin aşağıdaki kod parçasının ekran çıktısını göz önüne alalım.

```csharp
IEnumerator<IsBilgisi> IEnumerable<IsBilgisi>.GetEnumerator()
{
    yield return Isler[2];
    yield return Isler[1];
}
```

![mk128_5.gif](/assets/images/2005/mk128_5.gif)

Dikkat ederseniz foreach iterasyonumuz yield ile hangi elemanları hangi sırada döndürdüysek ona göre çalışmıştır.

Uygulamadaki gelişimi bir de IL kodu açısından düşünmek lazım. Aslında temelde numaralandırıcının kullanılması için gerekli olan IEnumerator bazlı bir inner class yine kullanılmaktadır. Bu sadece kod yazımında yapılmamaktadır. Öyleki, C# 1.1 için geliştirdiğimiz örneğin IL koduna ilDasm aracı ile bakarsak aşağıdaki ekran görüntüsünü yakalarız.

![mk128_4.gif](/assets/images/2005/mk128_4.gif)

Dikkat ederseniz burada Numarator isimli dahili sınıfımız (inner class) ve ona IEnumerator arayüzü vasıtasıyla uyguladığımız üyeler görülmektedir. Birde C# 2.0 için geliştirdiğimiz örneğin IL koduna bir bakalım.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Şu anki beta versiyonuna göre IlDasm aracının fiziki adresi \Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\ildasm.exe dır.

Dikkat ederseniz biz IEnumerator arayüzünü kullanarak bir dahili sınıfı yazmamış olsakta, IL kodunun detaylarında böyle bir yapının kullanıldığı görülmektedir. Tabi burada bir önceki versiyondan farklı olarak, generic tip uyarlamasıda mevcuttur.

![mk128_3.gif](/assets/images/2005/mk128_3.gif)

Artık uygulamamız için aşağıdaki kod parçasını başarılı bir şekilde çalıştırabiliriz.

```csharp
IsListesi isListesi = new IsListesi();
foreach (IsBilgisi i in isListesi)
{
    Console.WriteLine(i.IsAdi + " " + i.IsSorumlu);
}
```

![mk128_2.gif](/assets/images/2005/mk128_2.gif)

Gördüğünüz gibi C# 2.0 ile iteratif işlemlerin alt yapısının oluşuturlması C# 1.1 ' e göre daha az kod yazarak kolayca gerçekleştirilebiliyor. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.