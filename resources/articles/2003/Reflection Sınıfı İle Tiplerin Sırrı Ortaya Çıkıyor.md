---
title: "Reflection Sınıfı İle Tiplerin Sırrı Ortaya Çıkıyor"
pubDate: 2003-12-22 12:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - Reflection
  - DataTable
---

# Reflection Sınıfı İle Tiplerin Sırrı Ortaya Çıkıyor
Değerli Okurlarım, Merhabalar.

Hiç.NET ‘te yer alan bir tipinin üyelerini öğrenebilmek istediniz mi? Örneğin var olan bir.NET sınıfının veya sizin kendi yazmış olduğunuz yada bir başkasının yazdığı sınıfa ait tüm üyelerin neler olduğuna programatik olarak bakmak istediniz mi? İşte bugünkü makalemizin konusu bu. Herhangi bir tipe (type) ait üyelerin neler olduğunu anlayabilmek. Bu amaçla, Reflection isim uzayını ve bu uzaya ait sınıfları kullanacağız.

Bildiğiniz gibi.NET ‘te kullanılan her şey bir tipe aittir. Yani herşeyin bir tipi vardır. Üyelerini öğrenmek isteğimiz bir tipi öncelikle bir Type değişkeni olarak alırız. (Yani tipin tipini alırız. Bu nedenle ben bu tekniğe Tip-i-Tip adını verdim). Bu noktadan sonra Reflection uzayına ait sınıfları ve metodlarını kullanarak ilgili tipe ait tüm bilgileri edinebiliriz. Küçük bir Console uygulaması ile konuyu daha iyi anlamaya çalışalım. Bu örneğimizde, System.Int32 sınıfına ait üyelerin bir listesini alacağız. İşte kodlarımız;

```csharp
using System;
namespace ReflectionSample1
{
    class Class1
    {
        static void Main(string[] args)
        {
            Type tipimiz = Type.GetType("System.Int32");
            /* Öncelikle String sınıfının tipini öğreniyoruz. */
            System.Reflection.MemberInfo[] tipUyeleri = tipimiz.GetMembers();
            /* Bu satır ile, System.String tipi içinde yer alana üyelerin listesini Reflection uzayında yer alan, MemberInfo sınıfı tipinden bir diziye aktarıyoruz. */
            Console.WriteLine(tipimiz.Name.ToString() + " sınıfındaki üye sayısı:" + tipUyeleri.Length.ToString());
            /* Length özelliği, MemeberInfo tipindeki dizimizde yer alan üyelerin sayısını, (dolayısıyla System.String sınıfı içinde yer alan üyelerin sayısını) veriyor.*/
            /* İzleyen döngü ile, MemberInfo dizininde yer alan üyelerin birtakım bilgilerini ekrana yazıyoruz.*/
            for (int i = 0; i < tipUyeleri.Length; ++i)
            {
                Console.WriteLine(i.ToString() + ". üye adı:" + tipUyeleri[i].Name.ToString() + "||" + tipUyeleri[i].MemberType.ToString());
                /* Name özelliği üyenin adını verirken, MemberType özelliği ile, üyenin tipini alıyoruz. Bu üye tipi metod, özellik, yapıcı metod vb... dir.*/
            }
        }
    }
}
```

Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk24_1.gif](images/mk24_1.gif)

Şekil 1. System.Int32 sınıfının üyeleri.

Üye listesini incelediğimizde 15 üyenin olduğunu görürüz. Metodlar, alanlar vardır. Ayrıca dikkat ederseniz, Parse, ToString metodları birden fazla defa geçmektedir. Bunun nedeni bu metodların overload (aşırı yüklenmiş) versiyonlara sahip olmasıdır. Kodları incelediğimizde, System.Int32 sınıfına ait tipleri GetMembers metodu ile, System.Reflection uzayında yer alan MemberInfo sınıfı tipinden bir diziye aldığımızı görürüz. İşte olayın önemli kodları bunlardan oluşmaktadır. MemberInfo dışında kullanabaileceğimiz başka Reflection sınıflarıda vardır. Bunlar;

ConstructorInfo

Tipe ait yapıcı metod üyelerini ve bu üyelere ait bilgilerini içerir.

EventInfo

Tipe ait olayları ve bu olaylara ait bilgileri içerir.

MethodInfo

Tipe ait metodları ve bu metodlara ait bilgileri içerir.

FieldInfo

Tip içinde kullanılan alanları ve bu alanlara ilişkin bilgileri içerir.

ParameterInfo

Tip içinde kullanılan parametreleri ve bu parametrelere ait bilgileri içerir.

PropertyInfo

Tip içinde kullanılan özellikleri ve bu özelliklere ait bilgileri içerir.

Tablo 1. Reflection Uzayının Diğer Kullanışlı Sınıfları

Şimdi bu sınıflara ait örneklerimizi inceleyelim. Bu kez bir DataTable sınıfının üyelerini inceleyeceğiz. Örnek olarak, sadece olaylarını ve bu olaylara ilişkin özelliklerini elde etmeye çalışalım. Bu örneğimizde, yukarıda bahsettiğimiz tip-i-tip tekniğini biraz değiştireceğiz. Nitekim bu tekniği uygulamamız halinde bir hata mesajı alırız. Bunun önüne geçmek için, bir DataTable örneği (instance) oluşturup bu örneğin tipinden hareket edeceğiz. Dilerseniz hemen kodlarımıza geçelim.

```csharp
using System;

namespace ReflectionSample2
{
    class Class1
    {
        static void Main(string[] args)
        {
            System.Data.DataTable dt = new System.Data.DataTable(); /* Bir DataTable örneği(instance) yaratıyoruz.*/
            Type tipimiz = dt.GetType();
            /* DataTable örneğimizin GetType metodunu kullanarak, bu örneğin dolayısıyla DataTable sınıfının tipini elde ediyoruz. */
            System.Reflection.MethodInfo[] tipMetodlari = tipimiz.GetMethods();
            /* Bu kez sadece metodları incelemek istediğimizden, GetMethods metodunu kullanıyor ve sonuçları, MethodInfo sınıfı tipinden bir diziye aktarıyoruz.*/
            Console.WriteLine(tipimiz.Name.ToString() + " sınıfındaki metod sayısı:" + tipMetodlari.Length.ToString());
            /* Metod sayısını Length özelliği ile alıyoruz.*/
            /* Döngümüzü oluşturuyor ve Metodlarımızı bir takım özellikleri ile birlikte yazdırıyoruz.*/
            for (int i = 0; i < tipMetodlari.Length; ++i)
            {
                Console.WriteLine("Metod adi:" + tipMetodlari[i].Name.ToString() + " |Dönüş değeri:" + tipMetodlari[i].ReturnType.ToString());
                /* Metodun ismini name özelliği ile alıyoruz. Metodun dönüş tipini ReturnType özelliği ile aliyoruz. */
                System.Reflection.ParameterInfo[] prmInfo = tipMetodlari[i].GetParameters();
                /* Bu satırda ise, i indeksli metoda ait parametre bilgilerini GetParameters metodu ile alıyor ve Reflection uzayında bulunan ParameterInfo sınıfı tipinden bir diziye aktarıyoruz. Böylece ilgili metodun parametrelerine ve parametre bilgilerine erişebilicez.*/
                Console.WriteLine("-----Parametre Bilgileri-----" + prmInfo.Length.ToString() + " parametre");
                /* Döngümüz ile i indeksli metoda ait parametrelerin isimlerini ve tiplerini yazdırıyoruz. Bunun için Name ve ParameterType metodlarını kullanıyoruz.*/
                for (int j = 0; j < prmInfo.Length; ++j)
                {
                    Console.WriteLine("P.Adi:" + prmInfo[j].Name.ToString() + " |P.Tipi:" + prmInfo[j].ParameterType.ToString());
                }
                Console.WriteLine("----");
            }
        }
    }
}
```

Şimdi uygulamamızı çalıştıralım ve sonuçlarına bakalım.

![mk24_2.gif](images/mk24_2.gif)

Şekil 2. System.Data.DataTable tipinin metodları ve metod parametrelerine ait bilgiler.

Reflection teknikleri yardımıyla çalıştırdığımız programa ait sınıflarında bilgilerini elde edebiliriz. İzleyen örneğimizde, yazdığımız bir sınıfa ait üye bilgilerine bakacağız. Üyelerine bakacağımız sınıfın kodları;

```csharp
using System;

namespace ReflectionSample3
{
    public class OrnekSinif
    {
        private int deger;
        public int Deger
        {
            get
            {
                return deger;
            }
            set
            {
                deger =value;
            }
        }

        public string metod(string a)
        {
            return "Burak Selim SENYURT";
        }
        int yas = 27;
        string dogum = "istanbul";
    }
}
```

ikinci sınıfımızın kodları;

```csharp
using System;
using System.Reflection;

namespace ReflectionSample3
{
    class Class1
    {
        static void Main(string[] args)
        {
            Type tipimiz = Type.GetType("ReflectionSample3.OrnekSinif");
            MemberInfo[] tipUyeleri = tipimiz.GetMembers();
            for (int i = 0; i < tipUyeleri.Length; ++i)
            {
                Console.WriteLine("Uye adi:" + tipUyeleri[i].Name.ToString() + " |Uye Tipi:" + tipUyeleri[i].MemberType.ToString());
            }
        }
    }
}
```

Şimdi uygulamamızı çalıştıralım.

![mk24_3.gif](images/mk24_3.gif)

Şekil 3. Kendi yazdığımı sınıf üyelerinede bakabiliriz.

Peki kendi sınıfımıza ait bilgileri edinmenin bize ne gibi bir faydası olabilir. İşte şimdi tam dişimize gore bir örnek yazacağız. Örneğimizde, bir tablodaki verileri bir sınıf içersinden tanımladığımız özelliklere alacağız. Bu uygulamamız sayesinde sadece tek satırlık bir kod ile, herhangibir kontrolü veriler ile doldurabileceğiz. Bu uygulamada esas olarak, veriler veritabanındaki tablodan alınıcak ve oluşturduğumuz bir koleksiyon sınıfından bir diziye aktarılacak. Oluşturulan bu koleksiyon dizisi, bir DataGrid kontrolü ile ilişkilendirilecek. Teknik olarak kodumuzda, Reflection uzayının PropertyInfo sınıfını kullanarak, oluşturduğumuz sınıfa ait özellikler ile tablodaki alanları karşılaştıracak ve uygun iseler bu özelliklere tabloda karşılık gelen alanlar içindeki değerleri aktaracağız. Dilerseniz kodumuz yazmaya başlayalım.

```csharp
using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace ReflectDoldur
{
    /* Kitap sınıfı KitapKoleksiyonu isimli koleksiyon içinde saklayacağımız değerlerin tipi olucaktır ve iki adet özellik içerecektir. Bunlardan birisi, tablomuzdaki Adi alanının değerini, ikincisi ise BasimEvi alanının değerini tutacaktır.*/
    public class Kitap
    {
        private string kitapAdi;
        private string yayimci;
        /* Özelliklerimizin adlarının tablomuzdan alacağımız alan adları ile aynı olmasına dikkat edelim.*/
        public string Adi
        {
            get
            {
                return kitapAdi;
            }
            set
            {
                kitapAdi =value;
            }
        }
        public string BasimEvi
        {
            get
            {
                return yayimci;
            }
            set
            {
                yayimci =value;
            }
        }

        /* Yapıcı metodumuz parametre olarak geçirilen bir DataRow değişkenine sahip. Bu değişken ile o anki satırı alıyoruz.*/
        public Kitap(System.Data.DataRow dr)
        {
            PropertyInfo[] propInfos =this.GetType().GetProperties(); /* this ile bu sınıfı temsil ediyoruz. Bu sınıfın tipini alıp bu tipe ait özellikleri elde ediyor ve bunları PropertyInfo sınıfı tipinden diziye aktarıyoruz.*/
            /* Döngümüz ile tüm özellikleri geziyoruz. Eğer metodumuza parametre olarak geçirilen dataRow değişkenimiz, bu özelliğin adında bir alan içeriyorsa, bu özelliğe ait SetValue metodunu kullanarak özelliğimize, iligili tablo alanının değerini aktarıyoruz.*/
            for (int i = 0; i < propInfos.Length; ++i)
            {
                if (propInfos[i].CanWrite) /* Burada özelliğimizin bir Set bloğuna sahip olup olmadığına bakılıyor. Yani özelliğimizen yazılabilir olup olmadığına. Bunu sağlayan özelliğimiz CanWrite. Eğer özellik yazılabilir ise (yada başka bir deyişle readonly değil ise) true değerini döndürür.*/
                {
                    try
                    {
                        if (dr[propInfos[i].Name] != null) /* dataRow değişkeninde, özelliğimin adındaki alanın değerine bakılıyor. Null değer değil ise SetValue ile alanın değeri özelliğimize yazdırılıyor. */
                        {
                            propInfos[i].SetValue(this, dr[propInfos[i].Name], null);
                        }
                        else
                        {
                            propInfos[i].SetValue(this, null, null);
                        }
                    }
                    catch
                    {
                        propInfos[i].SetValue(this, null, null);
                    }
                }
            }
        }
    }

    /* KitapKoleksiyonu sınıfımız bir koleksiyon olucak ve tablomuzdan alacağımız iki alana ait verileri Kitap isimli nesnelerde saklanmasını sağlıyacak. Bu nedenle, bir CollectionBase sınıfından türetildi. Böylece sınıfımız içinde indeksleyiciler kullanabileceğiz.*/
    public class KitapKoleksiyonu : System.Collections.CollectionBase
    {
        public KitapKoleksiyonu()
        {
            SqlConnection conFriends =new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
            SqlDataAdapter da =new SqlDataAdapter("Select Adi,BasimEvi From Kitaplar", conFriends);
            DataTable dtKitap =new DataTable();
            da.Fill(dtKitap);
            foreach (DataRow drow in dtKitap.Rows)
            {
                this.InnerList.Add(new Kitap(drow));
            }
        }

        public virtual void Add(Kitap _kitap)
        {
            this.List.Add(_kitap);
        }
        public virtual Kitap this[int Index]
        {
            get
            {
                return (Kitap)this.List[Index];
            }
        }
    }
}
```

Ve işte formumuzda kullandığımız tek satırlık kod;

```csharp
private void btnDoldur_Click(object sender, System.EventArgs e)
{
     dataGrid1.DataSource =new ReflectDoldur.KitapKoleksiyonu();
} 
```

Şimdi uygulamamızı çalıştıralım ve bakalım.

![mk24_4.gif](images/mk24_4.gif)

Şekil 4. Programın Çalışmasının Sonucu

Görüldüğü gibi tablomuzdaki iki Alana ait veriler yazdığımız KitapKoleksiyonu sınıfı yardımıyla, her biri Kitap tipinde bir nesne alan koleksyionumuza eklenmiş ve bu sonuçlarda dataGrid kontrolümüze bağlanmıştır. Siz bu örneği dahada iyi bir şekilde geliştirebilirisiniz. Umuyorumki bu örnekte yapmak istediğimizi anlamışsınızdır. Yansıma tekniğini bu kod içinde kısa bir yerde kullandık. Sınıfın özelliklerinin isminin, tablodaki alanların ismi ile aynı olup olmadığını ve aynı iseler yazılabilir olup olmadıklarını öğrenmekte kullandık. Değerli Okurlarım. Geldik bir makalemizin daha sonuna. Hepinize mutlu günler dilerim.
