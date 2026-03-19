---
title: "Params Anahtar Sözcüğünün Kullanımı"
pubDate: 2003-11-30 12:00:00
categories:
  - C#
tags:
  - C#
  - DataSet
  - DataTable
---

# Params Anahtar Sözcüğünün Kullanımı
Değerli Okurlarım Merhabalar.

Bugünkü makalemizde, C# metodlarında önemli bir yere sahip olduğunu düşündüğüm params anahtar kelimesinin nasıl kullanıldığını incelemeye çalışacağız. Bildiğiniz gibi metodlara verileri parametre olarak aktarabiliyor ve bunları metod içersinde işleyebiliyoruz. Ancak parametre olarak geçirilen veriler belli sayıda oluyor. Diyelimki sayısını bilmediğimiz bir eleman kümesini parametre olarak geçirmek istiyoruz. Bunu nasıl başarabiliriz? İşte params anahtar sözcüğü bu noktada devreye girmektedir. Hemen çok basit bir örnek ile konuya hızlı bir giriş yapalım.

```csharp
using System; 
namespace ParamsSample1
{  
	class Class1
	{
		/* burada Carpim isimli metodumuza, integer tipinde değerler geçirilmesini sağlıyoruz. params anahtarı bu metoda istediğimiz sayıda integer değer geçirebileceğimizi ifade ediyor*/

		public int Carpim(params int[] deger)
		{
		int sonuc=1;  
			for(int i=0;i<deger.Length;++i) /*Metoda gönderilen elemanlar doğal olarak bir dizi oluştururlar. Bu dizideki elemanlara bir for döngüsü ile kolayca erişebiliriz. Dizinin eleman sayısını ise Length özelliği ile öğreniyoruz.*/
			{
				sonuc*=deger[i];
				/* Burada metoda geçirilen integer değerlerin birbirleri ile çarpılmasını sağlıyoruz*/
			}
			return sonuc;
		}
		static void Main(string[] args)
		{
			Class1 cl=new Class1();
			Console.WriteLine("1*2*3*4={0}",cl.Carpim(1,2,3,4));
			/* Burada Carpim isimli metoda 4 integer değer gönderdik. Aşağıdaki kodda ise 2 adet integer değer gönderiyoruz.*/
			Console.WriteLine("8*5={0}",cl.Carpim(8,5));
			Console.ReadLine();
		}
	}
}
```

Bu örneği çalıştıracak olursak, aşağıdaki sonucu elde ederiz.

![mk9_1.jpg](images/mk9_1.jpg)

Şekil 1. Ilk Params Örneğinin Sonucu

Peki derleyici bu işlemi nasıl yapıyor birazda ondan bahsedelim. Carpim isimli metoda değişik sayılarda parametre gönderdiğimizde, derleyici gönderilen paramtetre sayısı kadar boyuta sahip bir integer dizi oluşturur ve du dizinin elemanlarına sırası ile (0 indexinden başlayacak şekilde) gönderilen elemanları atar. Daha sonra aynı metodu bu eleman sayısı belli olan diziyi aktararak çağırır. cl.Carpim (8,5) satırını düşünelim; derleyici,

İlk adımda,

int[] dizi=new int[2] ile 2 elemanlı 1 dizi yaratır.

İkinci adımda,

dizi[0]=8

dizi[1]=5 şeklinde bu dizinin elemanlarını belirler.

Son adımda ise metodu tekrar çağırır.

cl.Carpim (dizi);

Bazı durumlarda parametre olarak geçireceğimiz değerler farklı veri tiplerine sahip olabilirler. Bu durumda params anahtar sözcüğünü, object tipinde bir dizi ile kullanırız. Hemen bir örnek ile görelim. Aynı örneğimize Goster isimli değer döndürmeyen bir metod ekliyoruz. Bu metod kendisine aktarılan değerleri console penceresine yazdırıyor.

```csharp
public void Goster(params object[] deger)
{   
	for(int i=0;i<deger.Length;++i)
    {
        Console.WriteLine("{0}. değerimiz={1}",i,deger[i].ToString());
    }
    Console.ReadLine();
}
static void Main(string[] args)
{
    cl.Goster(1,"Ahmet",12.3F,0.007D,
	true,599696969,"C");
}
```

Görüldüğü gibi Goster isimli metodumuza değişik tiplerde (int,Float,Decimal,bool, String) parametreler gönderiyoruz. İşte sonuç;

![mk9_2.jpg](images/mk9_2.jpg)

Şekil 2. params object[] kullanımı.

Şimdi dilerseniz daha işe yarar bir örnek üzerinde konuyu pekiştirmeye çalışalım. Örneğin değişik sayıda tabloyu bir dataset nesnesine yüklemek istiyoruz. Bunu yapıcak bir metod yazalım ve kullanalım. Programımız, bir sql sunucusu üzerinde yer alan her hangibir database’e bağlanıp istenilen sayıdaki tabloyu ekranda programatik olarak oluşturulan dataGrid nesnelerine yükleyecek. Kodları inceledikçe örneğimizi daha iyi anlıyacaksınız.

![mk9_3.jpg](images/mk9_3.jpg)

Şekil 3. Form Tasarımımız

Uygulamamız bir Windows Application. Bir adet tabControl ve bir adet Button nesnesi içeriyor. Ayrıca params anahtar sözcüğünü kullanan CreateDataSet isimli metodumuzu içeren CdataSet isimli bir class’ımızda var. Bu class’a ait kodları yazarak işimize başlayalım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient; 
namespace CreateDataSet
{    
	public class CDataSet
    {
/* CreateDataSet isimli metod gönderilen baglantiAdi stringinin değerine göre bir SqlConnection nesnesi oluşturur. tabloAdi ile dataset nesnesine eklemek istediğimi tablo adlarini bu metoda göndermekteyiz. params anahtarı kullanıldığı için istediğimiz sayıda tablo adı gönderebiliriz. Elbette, geçerli bir Database ve geçerli tablo adları göndermeliyiz.*/
		public DataSet CreateDataSet(string baglantiAdi,params string[] tabloAdi)
        {
			string sqlSelect,conString;
            conString="data source=localhost;initial catalog="+baglantiAdi+";integrated security=sspi";

			/* Burada SqlConnection nesnesinin kullanacağı connectionString'i belirliyoruz.*/
            DataSet ds=

			new DataSet();/* Tablolarimizi taşıyacak dataset nesnesini oluşturuyoruz*/
            SqlConnection con=new SqlConnection(conString); /*SqlConnection nesnemizi oluşturuyoruz*/
            SqlDataAdapter da;

			/* Bir SqlDataAdapter nesnesi belirtiyoruz ama henüz oluşturmuyoruz*/ 
			/*Bu döngü gönderdiğimiz tabloadlarını alarak bir Select sorgusu oluşturur ve SqlDataAdapter yardımıyla select sorgusu sonucu dönen tablo verilerini oluşturulan bir DataTable nesnesine yükler. Daha sonra ise bu DataTable nesnesi DataSet nesnemizin Tables kolleksiyonuna eklenir. Bu işlem metoda gönderilen her tablo için yapılacaktır. Böylece döngü sona erdiğinde, DataSet nesnemiz göndermiş olduğumuz tablo adlarına sahip DataTable nesnelerini içermiş olucaktır. */
			for(int i=0;i<tabloAdi.Length;++i)
            {
                sqlSelect="SELECT * FROM "+tabloAdi[i];
                da=new SqlDataAdapter(sqlSelect,con);
                DataTable dt=new DataTable(tabloAdi[i]);
                da.Fill(dt);
				ds.Tables.Add(dt);
            } 
			return ds; /* Son olarak metod çağırıldığı yere DataSet nesnesini göndermektedir.*/
        } 
		public CDataSet()
        {
        }
    }    
} 
```

Şimdi ise btnYukle isimli butonumuzun kodlarını yazalım.

```csharp
private void btnYukle_Click(object sender, System.EventArgs e)
{
    CDataSet c=new CDataSet();
    DataSet ds=new DataSet();
    ds=c.CreateDataSet("northwind","Products","Orders");
	for(int i=0;i<ds.Tables.Count;++i)
    {        
		/* tabControl'umuza yeni bir tab page ekliyoruz.*/
        tabControl1.TabPages.Add(new System.Windows.Forms.TabPage(ds.Tables[i].TableName.ToString()));
        /* Oluşturulan bu tab page'e eklenmek üzere yeni bir datagrid oluşturuyoruz.*/
        DataGrid dg=new DataGrid();
        dg.Dock=DockStyle.Fill;

		/*datagrid tabpage'in tamamını kaplıyacak*/
        dg.DataSource=ds.Tables[i];

		/* DataSource özelliği ile DataSet te i indexli tabloyu bağlıyoruz.*/ 
        tabControl1.TabPages[i].Controls.Add(dg);

		/* Oluşturduğumuz dataGrid nesnesini TabPage üstünde göstermek için Controls koleksiyonunun Add metodunu kullanıyoruz.*/
    }
}
```

Şimdi programımızı çalıştıralım. İşte sonuç;

![mk9_4.jpg](images/mk9_4.jpg)

Şekil 4. Tabloların yüklenmesi.

Görüldüğü gibi iki tablomuzda yüklenmiştir. Burada tablo sayısını arttırabilir veya azaltabiliriz. Bunu params anahtar kelimesi mümkün kılmaktadır. Örneğin metodomuzu bu kez 3 tablo ile çağıralım;

```csharp
ds=c.CreateDataSet("northwind","Products","Orders","Suppliers");
```

Bu durumda ekran görüntümüz Şekil 5 teki gibi olur.

![mk9_5.jpg](images/mk9_5.jpg)

Şekil 5. Bu kez 3 tablo gönderdik.

Umuyorumki params anahtar sözcüğü ile ilgili yeterince bilgi sahibi olmuşsunuzdur. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
