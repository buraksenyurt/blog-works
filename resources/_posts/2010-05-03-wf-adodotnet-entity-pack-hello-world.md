---
layout: post
title: "WF Ado.Net Entity Pack - Hello World"
date: 2010-05-03 01:00:00 +0300
categories:
  - wf-4-0
tags:
  - workflow-foundation
  - wf-ado.net-activity-pack
  - wf-state-machine-activity-pack
  - activity
---
2008 yılının son çeyreğinde Microsoft tarafından düzenlenen Yazılım Geliştiriciler Zirvesinde, WCF & WF 4.0 konulu bir sunumum olmuştu. Derinlere Dalıyoruz mesajını içeren etkinliğin sunum dosyalarının arka plan resminde, dipteki bilgisayara ulaşmaya çalışan bir balık adam motifi yer almaktaydı. Sevgili Mehmet Emre'nin beni davet ettiği bu oturuma hazırlanırken, Microsoft PDC 2008' de dağıtılan VHD'ler üzerinde çalışmıştım. O zamanlarda WF Designer üzerinde dikkatimi çeken noktaların başında, WF aktivitelerindeki çeşitlilik yer almaktaydı. Özellike DbQuery, DbUpdate isimli SQL odaklı aktivite bileşenleri dikkatimi çekmişti.

![blg191_Giris.gif](/assets/images/2010/blg191_Giris.gif)

Ne var ki ilerleyen zamanlarda çıkan PreBeta, Beta 1, Beta 2, RC ve nihayet RTM sürümlerinde yer alan Activity Component setinde bu tip bileşenlerin yer almadığına da şahit olduk. Hatta WF 4.0 öncesinde aşina olduğumuz State Machine tipinden şablonlarında kaldırıldığını gördük. Geçtiğimiz günlerde ise Codeplex üzerinden iki WF 4.0 Activity Pack yayınlandı. Bunlardan birisi [WF Ado.Net Activity Pack CTP 1](http://wf.codeplex.com/releases/view/43585) iken diğer ise [WF State Machine Activity Pack 1](http://wf.codeplex.com/releases/view/43586) isimli paketti.

Söz konusu paketleri indirip kurduğumuzda Visual Studio 2010 Workflow Designer'a aşağıdaki ekran görüntüsünde yer alan kontrollerin eklendiğini fark edebiliriz.

![blg191_Components.gif](/assets/images/2010/blg191_Components.gif)

İşte bu yazımızda çok basit olarak WF Ado.Net Activity Pack içerisindeki kontrollerden birisini tanımaya çalışacak ve basit bir örnek geliştiriyor olacağız. Bu tip Pack'leri işlerimizi son derece kolaylaştıracak ve Activity bileşenlerinin daha zengin bir çerçevede değerlendirilmesine olanak sağlayacak genişletmeler olarak düşünmemizde yarar vardır. Özellikle Codeplex gibi açık kaynak kodlu projeleri yayınlayan ve Microsoft tarafından desteklenen bu tip bileşen paketleri, kişisel düşünceme göre WF tasarımlarında önemli bir rol üstlenecekler gibi görünmekte. Haydi gelin parmaklarımızı sıvayalım ve basit bir Workflow Console Application projesi oluşturarak yola koyulalım. İlk olarak senaryomuzdan bahsetmemizde yarar olacağı kanısındayım.

Örneğimizde bir Select SQL sorgusunu, SQL 2008 sunucusu üzerinde konuşlandırılmış ve yine Codeplex sitesinden indirip kurabileceğiniz Chinook veritabanı üzerindeki Track tablosunda çalıştırıyor olacağız. Bu sorgunun çalıştırılması sonucu elde edilen veri kümesi (Result Set) içerisindeki her bir satırı ise, aşağıda tasarımı görülen Track isimli sınıfa ait nesne örneklerinin oluşturulmasında kullanacağız.

```csharp
namespace HelloAdoNetActivityPack
{
    public class Track
    {
        public int TrackId { get; set; }
        public string Name { get; set; }
        public string Composer { get; set; }
        public int Milliseconds { get; set; }

        public override string ToString()
        {
            return string.Format("{0}  Şarkı : {1} Besteci : {2} Süre : {3} ", TrackId.ToString(), Name, Composer, Milliseconds.ToString());
        }
    }
}
```

Şimdi Workflow ortamımızda Sequence diagramı içerisine bir adet ExecuteSqlQuery tipinden bileşeni sürükleyelip bırakalım. Sürükleme işlemi sonrasında T tipi için aşağıdaki ekran görüntüsünde yer alan soru ile karşılaşırız.

![blg191_SelectTypes.gif](/assets/images/2010/blg191_SelectTypes.gif)

Çok doğal olarak sorgu sonuçlarının Track tipinden nesne örnekleri içerisinde toplanmasını planladığımızdan bu sınıfı seçmemiz gerekmektedir. Browse for Types kısmından Track sınıfını işaretledikten sonra kontrolümüzün WF Designer ortamına aşağıdaki gibi eklendiğini görebiliriz.

![blg191_FirstState.gif](/assets/images/2010/blg191_FirstState.gif)

Eğer bu tip bir Activity bileşenini kendimiz tasarlıyor olsaydık, çok doğal olarak sorgunun çalıştırılması için gerekli bağlantıyı ve sorgunun kendisini birer özellik (Property) olarak sunmayı planlardık. Benzer durum ExecuteSqlQuery bileşeni için de geçerlidir. Bir başka deyişle ConnectionString, CommandText ve CommandType özelliklerinin bildirilmesi gerekmektedir. Tam bu noktada söz konusu bileşenin SqlCommand nesne örneğinin görsel bir formatı olduğunu da ifade edebiliriz.

Özellikle CommandType, ProviderName özellikleri aldığı değerler ile bunu ispat eder niteliktedir. CommandType özelliğine Text, StoredProcedure, TableDirect değerlerinden birisini verebiliriz. Buna göre bir Stored Procedure'ün, Tablo adının veya bizim tarafımızdan yazılacak bir SQL Sorgusunun kullanılabilmesi mümkündür. Ayrıca parametreli sorgulamalar için Parameters özelliği göz önüne alınabilmektedir. Standart olarak SqlClient Veri Sağlayıcısı (Data Provider) kullanılmaktadır. Ancak dilerseniz diğer Provider tiplerini de seçebilir ve farklı veri kaynakları için gerekli sorguları yürütebilirsiniz. ConnectionString, Parameters ve CommandText vb özelliklerinin yanında bulunan üç nokta düğmeleri, sağladıkları arabirimler ile ilgili özelliklerin kolayca belirlenebilmesini sağlamaktadır.

Örneğimizde ConnectionString için aşağıdaki ayarlar kullanılmaktadır.

![blg191_Connection.gif](/assets/images/2010/blg191_Connection.gif)

CommandText özelliğini ise aşağıdaki gibi belirlediğimizi düşünebiliriz.

![blg191_CommandTezxt.gif](/assets/images/2010/blg191_CommandTezxt.gif)

Buna göre AlbumId değerine göre Track bilgilerini getiren bir SQL sorgusunu çalıştırıyor olacağız. SQL sorgusunda @AlbumId isimli bir parametre kullandığımızdan bunun değerini ortama alabilmek amacıyla bir Argument örneğinden yararlanıyor olacağız. Söz konusu parametre bildirimini ise aşağıdaki gibi yaptığımızı düşünebiliriz.

![blg191_Parameters.gif](/assets/images/2010/blg191_Parameters.gif)

Buraya kadar her şey güzel. Peki elde ettiğimiz veri kümesini nasıl değerlendireceğiz? Aslında ExecuteSqlQuery kontrolü sorgu sonuçlarının SqlDataReader yardımıyla dolaşılmasına olanak sağlamaktadır. Bu nedenle Map Each record to target gibi bir alt içeriğe sahiptir. Biz de buradaki alanda örnek olarak bir Assign aktivitesi kullanacak ve target nesne örneklerini oluşturacağız. Burada söz konusu olan target değişkeni T tipindendir. Dolayısıyla Track sınıfını işaret etmektedir. Diğer yandan record değişkeni ise, sorgu sonucu okunmakta olan her bir satırı ifade etmektedir. Bu bilgiler ışığında kontrolün içeriğini ve Assign aktivitesinin To özelliğini aşağıdaki Visual Basic Expression ile tamamlayabiliriz. (Bildiğiniz üzere WF tarafındaki tüm Expression'lar Visual Basic Syntax ' ında yazılmaktadır)

![blg191_Record.gif](/assets/images/2010/blg191_Record.gif)

Görüldüğü üzere Track nesne örneğini oluştururken record değişkeni üzerinden ilgili alan değerleri (Field Values) belirlenmiş ve ilgili özelliklere (Property) atanmıştır.

> Örneği geliştirirken C1020: Build error occurred in the XAML MSBuild task: ''xml:space'is a duplicate attribute name içerikli bir derleme zamanı hatası ile karşılaştım. Bunun üzerine aşağıdaki XAML içeriğinde yer alan xml:space="preserve" kısmını kaldırarak hatanın önüne geçtim. Söz konusu hatanın sebebinin araştırmaya devam ediyorum.

```csharp
<InArgument x:TypeArguments="local:Track" xml:space="preserve">[New Track() With {
    .TrackId = record.GetInt32(0),
    .Name = record.GetString(1),
    .Composer = record.GetString(2),
    .Milliseconds = record.GetInt32(3)
    }]</InArgument>
```

ExecuteSqlQuery kontrolünün yürüttüğü sorgu sonuçlarını aslında List tipinden bir koleksiyonda toplanmaktadır. Dolayısıyla sorgu sonuçları örneğimize göre List tipinden bir koleksiyon içerisine de aktarılmaktadır. Bu sorgu sonucu da List tipinden bir Variable veya Argument'a atanarak kullanılabilir. Tabi bunun için ExecuteSqlQuery bileşeninin Result özelliğinin uygun değişkene atanarak kontrolün dış ortamına sunulması gerekmektedir.

![blg191_Result.gif](/assets/images/2010/blg191_Result.gif)

Artık basit bir ForEach aktivitesini kullanarak elde edilen veri kümesi üzerinden hareket edebiliriz. Buna göre Workflow1 örneğinin son hali aşağıdaki şekilde görüldüğü gibi tamamlanabilir.

![blg191_DesignerLast.gif](/assets/images/2010/blg191_DesignerLast.gif)

ForEach aktivite bileşeni de Track tipi ile çalışacak şekilde tesis edilmiş ve bu sayede ExecuteSqlQuery aktivitesinin ürettiği List tipinden olan koleksiyon üzerinde dolaşmak üzere ayarlanmıştır. Olayı çok basit bir şekilde ele almak için sadece bir WriteLine aktivitesi kullanılmış ve varsayılan değeri 1 olan AlbumId değerli Track örneklerinin içeriği ekrana yazdırılmıştır. İşte çalışma zamanı sonuçları.

![blg191_Runtime.gif](/assets/images/2010/blg191_Runtime.gif)

Süper

![Wink](/assets/images/2010/smiley-wink.gif)

Görüldüğü üzere SQL sorgu cümlelerinin icra edilmesi ve özellikle üretilen veri kümesinin Workflow tarafında ele alınması oldukça kolay bir hale getirilmiştir. Örneğimizin Sequence bileşenine ait XAML çıktısı aşağıdaki gibidir.

```xml
<Sequence sad:XamlDebuggerXmlReader.FileName="D:\Vs 2010\RTM\Workflow Foundation\HelloAdoNetActivityPack\HelloAdoNetActivityPack\Workflow1.xaml" sap:VirtualizedContainerService.HintSize="309,637">
    <Sequence.Variables>
      <Variable x:TypeArguments="scg3:List(local:Track)" Name="TracksByAlbumId" />
    </Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <mda:ExecuteSqlQuery x:TypeArguments="local:Track" CommandText="Select TrackId,Name,Composer,Milliseconds From Track Where AlbumId=@AlbumId" ConnectionString="Data Source=.;Initial Catalog=Chinook;Integrated Security=True" sap:VirtualizedContainerService.HintSize="287,267" ProviderName="System.Data.SqlClient" Result="[TracksByAlbumId]">
      <mda:ExecuteSqlQuery.RecordProcessor>
        <ActivityFunc x:TypeArguments="sd:IDataRecord, local:Track">
          <ActivityFunc.Argument>
            <DelegateInArgument x:TypeArguments="sd:IDataRecord" Name="record" />
          </ActivityFunc.Argument>
          <ActivityFunc.Result>
            <DelegateOutArgument x:TypeArguments="local:Track" Name="track" />
          </ActivityFunc.Result>
          <Assign sap:VirtualizedContainerService.HintSize="252,100">
            <Assign.To>
              <OutArgument x:TypeArguments="local:Track">[track]</OutArgument>
            </Assign.To>
            <Assign.Value>
              <InArgument x:TypeArguments="local:Track">[New Track() With { .TrackId = record.GetInt32(0), .Name = record.GetString(1), .Composer = record.GetString(2), .Milliseconds = record.GetInt32(3) }]</InArgument>
            </Assign.Value>
          </Assign>
        </ActivityFunc>
      </mda:ExecuteSqlQuery.RecordProcessor>
      <InArgument x:TypeArguments="x:Int32" x:Key="AlbumId">1</InArgument>
    </mda:ExecuteSqlQuery>
    <ForEach x:TypeArguments="local:Track" DisplayName="ForEach<Track>" sap:VirtualizedContainerService.HintSize="287,206" Values="[TracksByAlbumId]">
      <ActivityAction x:TypeArguments="local:Track">
        <ActivityAction.Argument>
          <DelegateInArgument x:TypeArguments="local:Track" Name="trck" />
        </ActivityAction.Argument>
        <WriteLine sap:VirtualizedContainerService.HintSize="257,100" Text="[trck.ToString()]" />
      </ActivityAction>
    </ForEach>
  </Sequence>
```

Özellikle ExecuteSqlQuery tipinin bold olarak işaretlenmiş nitelikleri ve değerleri bizim için dikkat edilmesi gereken noktalar arasında yer almaktadır. Bundan sonraki kısımda size düşenleri ise şu şekilde özetleyebiliriz.

- Aynı örneği bir Stored Procedure kullanacak ve parametre değerlerini çalışma zamanında (Runtime) kullanıcıdan alacak hale getirmeyi deneyebilirsiniz. Ayrıca sorgu sonuçlarını Workflow dışına çıkartmak için gerekli hamleleri yapmayı deneyebilirsiniz (Argument kullanımı)
- Bu yazımızda değinmediğimiz ExecuteSqlNonQuery Activity bileşeninin ne amaçla kullanıldığını incelemeye çalışabilirsiniz.
- ExecuteSqlQuery bileşeninin CommandType özelliğinden yararlaran doğrudan Tablo adını belirtmek suretiyle ilgili sonuç kümesini değerlendirmeye çalışabilirsiniz.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HelloAdoNetActivityPack.rar (73,63 kb)](/assets/files/2010/HelloAdoNetActivityPack.rar)
