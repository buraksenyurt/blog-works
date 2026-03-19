---
title: "Data Form Wizard Yardımıyla İlişkili Tablo Uygulamalarının Hazırlanması"
pubDate: 2004-02-09 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - SQL Server
  - Authentication
  - Visual Studio
  - DataSet
  - DataTable
---

# Data Form Wizard Yardımıyla İlişkili Tablo Uygulamalarının Hazırlanması
Değerli Okurlarım, Merhabalar.

Bu makalemizde, Visual Studio.NET ortamında, Data Form Wizard yardımıyla, veritabanı uygulamalarının ne kadar kolay bir şekilde oluşturulabileceğini inceleyeceğiz. Pek çok programcı, uygulamalarını geliştirirken sihirbazları kullanmaktan açıkça kaçınır. Bunun bir nedeni, sihirbazların işlemleri çok fazla kolaylaştırması ve programcıyı tembelliğe itmesidir. Gerçektende, bir Data Form Wizard yardımıyla uzun sürede programlayacağınız bir veritabanı uygulamsını inanılmaz kısa sürede tamamlayabilirisiniz. Diğer yandan, bir programcı için bir uygulamayı geliştirmekteki en önemli unsur belkide şu kelimenin arkasında gizlidir; Kontrol.

Kontrol bir programcı için, uygulamanın her yerinde hakim olmak demektir. Yazılabilecek her kodun programcı tarafından yazılması, olabilecek tüm hataların düzeltilmesi, mantıksal bütünlüklerin sağlanması ve kullanıcının ihtiyaçlarına en üst düzeyde cevap verilebilmesi, programcının kontrolünü güçlendiren unsurlar arasında yer alır. Gerçektende ben size, gerçek hayatta sihirbazları çok fazla kullanmamanızı tavsiye ederim. Herşeyden önce, sihirbazlar tek düzelik sağlarlar ve sürekli aynı adımları atarlar. Bu bir süre sonra hem sizi tembelleştirecek hemde gerçek bir programcı gibi düşünmekten örneğin oluşabilecek programatik hataların önceden sezilebilmesi yetenğinden mahrum bırakacaktır.

Ancak tüm bu olumsuzlukların yanında, bir Data Form Wizard aracını kullanaraktan,.NET ortamında veritabanı programlamasını öğrenmeye çalışan bir programcı için, neler yapılabileceği?, bunların hangi temel adımlarla gerçekleştirildiği?, hatta Visual Studio.NET tarafından otomatik olarak oluşturulan kodların nerelerde devreye girdiği? gibi sorunların anlaşılmasıda oldukça kolay olucaktır. Diğer yandan profesyonel programcılarda zaman zaman, sadece kendileri için bir takım verileri kolayca izleyebilmek amacıyla yada ani müdahalelerde bulunmak amacıyla oluşturacakları uygulamalarda aynı kodları tekrardan yazmak yerine sihirbazları kullanmayı tercih edebilirler. Dolayısıyla zaman zaman sihirbazları kullanmak oldukça işe yarayabilir ama bunu alışkanlık halinede getirmemek gerekir. İşte bu makalemizde, bir Ado.Net uygulamasının, ilişkili iki tablo için nasıl kolayca oluşturulabileceğini incelyeceğiz. Uygulamamız sona erdiğinde, tabloları izleyebilecek, ilişkili kayıtları inceleyebilecek, yeni kayıtlar ekleyebilecek, var olanları düzenleyip silebilecek ve kayıtlar arasında gezinebileceğiz. Ayrıca sihirbazımız, programatik olarak oluşturulan Ado.Net uygulamalarında da hangi adımları takip edeceğimiz hakkında bize kılavuzluk etmiş olucak. Şimdi dilerseniz uygulamamızı yazmaya başlayalım. Öncelikle Visual Studio ortamında bir C# Windows Uygulaması açalım. Daha sonra veritabanı uygulamamızı taşıyacak formu eklemek için, Solution sekmesinde, projemize sağ tuş ile tıklayalım ve Add New Item öğesini seçelim.

![mk52_1.gif](images/mk52_1.gif)

Şekil 1. Add New Item.

Bu durumda karşımıza aşağıdaki pencere gelecektir. Bu pencerede Data Form Wizard aracını seçelim. Data Form Wizard aracı tüm adımları bitirildikten sonra, projemize cs uzantılı bir sınıf ve bu sınıfı kullanan bir Form ekleyecektir. Veritabanına bağlanılması, tabloların dolduruluması, satırlar arasında gezinme gibi pek çok işlevi yerine getiren metodlar, özellikler ve nesneler, Data Form Wizard ile oluşturulan bu sınıf içerisine yazılacaktır. Burada Form dosyanıza anlamlı bir isim vermenizi öneririm.

![mk52_2.gif](images/mk52_2.gif)

Şekil 2. Data Form Wizard

Bundan sonra, artık uygulamayı oluşturacağımız adımlara geçmiş oluruz.

![mk52_3.gif](images/mk52_3.gif)

Şekil 3. Adımlara başlıyoruz.

İlk adımımız DataSet nesnemizin isimlendirilmesi. Bir DataSet, veritabanından bağımsız olarak uygulamanın çalıştığı sistemin belleğinde oluşturulan bir alana referans eden kuvvetli bir nesnedir. Bir DataSet içersine, tablolar, tablolar arası ilişkiler vb. ekleyebiliriz. Bir DataSet ile, veritabanındanki gerekli veriler DataSet nesnesinin bellekte temsil ettiği bölgeye yüklendikten sonra bu veritabanına bağlı olmaksızın çalışabiliriz. Dahada önemlisi, veritabanına bağlı olmaksızın, veriler ekleyebilir, verileri düzenleyebilir, silebilir, yeni tablolara, görünümler, ilişkiler oluşturabiliriz. DataSet, ADO.NET 'in en önemli yeniliklerinden birisi olmakla birlikte, bağlantısız katman dediğimiz kısımın bir parçasıdır. DataSet'i ilerleyen makalelerimizde daha detaylı olarak da inceleyeceğiz. Şu an için bilmeniz gereken, oluşturacağımız DataSet'in Sql sunucumuzda (veya başka bir databasede) yer alan ilişkili tablolarımızı ve aralarındaki ilişkiyi içerecek olan bağlantısız (ADO'daki RecordSet gibi, Veritabanına sürekli bir bağlantıyı gerektirmeyen) bir nesne oluşudur. Şimdi burada uygulamaya önceden eklediğimiz bir DataSet nesnesini seçebileceğimiz gibi yeni bir tanede oluşturabiliriz. Biz yeni bir DataSet oluşturacağımızdan, Creat a new database named alanına, DataSet'imizin adını giriyoruz.

![mk52_4.gif](images/mk52_4.gif)

Şekil 4. İlk adımımız DataSet nesnemizi belirleyeceğimiz ad ile oluşturmak.

Sıradaki adım, veritabanına olan bağlantımızı gerçekleştirecek nesnemizi tanımlayacak. Herşeyden önce DataSet nesnemize bir veritabanından veriler yükleyeceksek, bir veri sağlayıcısı üzerinden bu verileri çekmemiz gerekecektir. İşte bu amaçla, pek çok ADO.NET nesnesi için gerekli olan ve belirtilen veri sağlayıcısı üzerinden ilgili veri kaynağına bir bağlantı açmamıza yarayan bir bağlantı (Connection) nesnesi kullanacağız. Burada uygulamamızda Sql Sunucumuzda yer alan veritabanına OleDb üzerinden erişmek istediğimiz için, sihirbazımız bize bir OleDbConnection nesnesi örneği oluşturacaktır. Veri sağlayıcısının tipine göre, OleDbConnection, SqlConnection, OracleConnection, OdbcConnection bağlantı nesnelerini oluşturabiliriz. New Connection seçeneğini seçerek yeni bir bağlantı oluşturmamızı sağlayacak diğer bir sihirbaza geçiş yapıyoruz.

![mk52_5.gif](images/mk52_5.gif)

Şekil 5. Connection nesnemizi oluşturmaya başlıyoruz.

Şimdi karşımızda Data Link Properties penceresi yer almakta. Burada, Provider kısmında Microsoft OLE DB Provider For Sql Server seçeneği seçili olmalıdır.

![mk52_6.gif](images/mk52_6.gif)

Şekil 6. Provider.

Connection sekmesinde ise öncelikle bağlanmak istediğimiz sql sunucusunun adını girmeliyiz. Sql sunucumuz bu uygulamada, istemci bilgisayar üzerinde kurulu olduğundan buraya localhost yazabiliriz. 2nci seçenekte, sql sunucusuna log in olmamız için gerekli bilgileri veriyoruz. Eğer sql sunucumuz windows authentication'ı kullanıyorsa, Use Windows Integrated Security seçeneğini seçebiliriz. Bu durumda sql sunucusuna bağlanmak için windows kullanıcı bilgileri kontrol edilecektir. Bunun sonucu olarak username ve password kutucukları geçersiz kılınıcakatır. Diğer yandan sql sunucusuna, burada oluşturulmuş bir sql kullanıcı hesabı ilede erişebiliriz. Ben uygulamamda sa kullanıcısını kullandım. Bildiğiniz gibi sa sql kullanıcısı admin yetkilerine sahip bir kullanıcı olarak sql sunucusunun kurulması ile birlikte sisteme default olarak yüklenir. Ancak ilk yüklemede herhangibir şifresi yoktur. Güvenlik amacıyla bu kullanıcıya mutlaka bir şifre verin.(Bunu sql ortamında gerçekleştireceksiniz.) Allow Saving Password seçeneğini işaretlemediğimiz takdirde, uygulamayı kim kullanıyor ise, uygulama başlatıldığında sql sunucusuna bağlanabilmek için kendisinden bu kulllanıcı adına istinaden şifre sorulacaktır. Eğer bu seçeneği işaretlersek, program çalıştırıldığında buraya girilen şifre sorulmayacaktır. Şimdi 3nci seçeneğe geçersek ve combo box kontrolünü açarsak sql sunucusunda yer alan veritabanlarının tümünü görebiliriz. Buradan kullanacağımız veritabanını seçeceğiz.

![mk52_7.gif](images/mk52_7.gif)

Şekil 7. Connection bilgileri giriliyor.

Dilersek Test Connection butonuna tıklayarak, sql sunucusuna bağlanıp bağlanamadığımızı kontrolde edebiliriz. Şimdi OK diyelim ve Data Link Properties penceresini kapatalım. Bu durumda karşımıza bir Login penceresi çıkacaktır. Bu az önce Allow Saving Password seçeneğini işaretlemediğimiz için karşımıza gelmiştir. Şifremizi tekrardan girerek bu adımı geçiyoruz. Bu durumda, sql sunucumuz için gerekli bağlantıyı sağlıyacak connection nesnemizde oluşturulmuş olur.

![mk52_8.gif](images/mk52_8.gif)

Şekil 8. Login penceresi.

Sıradaki adımımızda ise, uygulamamızda kullanacağımız tabloları seçeceğiz. Bu adımda, bağlandığımız veritabanında yer alan tablolar ve görünümler yer almaktadır. Biz hemen ekleyeceğimiz iki tabloyu seçip pencerenin sağ tarafında yer alan Seleceted Item (s) kısmına aktarıyoruz. Bu tabloların her biri birer DataTable nesnesi olucak şekilde DataSet nesnemize eklenir. DataTable nesneleri tablolara ait bilgilerinin ve verilerinin bellekte yüklendiği belli bir alanı referans ederler. Bir DataSet nesnesi birden fazla DataTable içerebilir. Böylece veritabanı ortamını DataSet nesnesi ile uygulamanın çalıştığı sistem belleğinde simule etmiş oluruz.

![mk52_9.gif](images/mk52_9.gif)

Şekil 9. Tablolar seçiliyor.

Bu iki tablomuz hakkında yeri gelmişken kısaca bilgi verelim. Sepetler tablosunda alışveriş sepetlerine ait bilgiler yer alıyor. Siparsler tablosu ise bu belirli sepetin içinde yer alan ürünlerin listesini tutuyor. Yani Sepetler tablosundan Siparisler tablosuna doğru bire-çok ilişki söz konusu. Bu adımıda tamamladıktan sonra, veri kaynağına log in olmamız için tekrar bir şifre ekranı ile karşılacağız. Burayıda geçtikten sonra sıra, iki tablo arasındaki ilişkiyi kuracağımız adıma geldi. Burada aralarında ilişki oluşturmak istediğimiz tabloları ve ilgili kolonları seçerek ilişkimiz için bir isim belirleyeceğiz. Bunun sonucu olarak sihirbazımız bir DataRelation nesnesi oluşturacak ve bu nesneyi DataSet'e ekliyecek. Böylece veritabanından bağımsız olarak çalışırken, DataSet kümemiz bu DataRelation nesnesini kullanarak tablolar arasındaki bütünlüğüde sağlamış olucak. Öncelikle bu ilişki için anlmalı bir ad belirleyin. Çoğunlukla ilişkinin hangi taboladan hangi tabloya doğru olduğuna bağlı olunarak bir isimlendirme yapılır. Daha sonra Parent (Master) tablo ve Primary Key seçilir. Ardından Foreign Key alanını içeren tabloda, Child (Detail) tablo olarak seçilir. Uyulamamızda Sepetler tablosu ile Siparisler tablosu SepetID alanları ile birbirlerine ilişkilendirilmiştir.

![mk52_10.gif](images/mk52_10.gif)

Şekil 10. DataRelation nesnesi oluşturuluyor.

Bir sonraki adımımızda tabloların hangi alanlarının Form üzerinde görüntüleneceğini belirleriz. Başlangıç için bütün alanlar seçili haldedir. Bizde bunu uygulamamızda bu halde bırakacağız.

![mk52_11.gif](images/mk52_11.gif)

Şekil 11. Görüntülenecek Alanların Seçilmesi.

Son adım ise, Formun tasarımının nasıl olacağıdır. İstersek master tabloyu DataGrid kontrolü içinde gösterebiliriz. Ya da Master tabloya ait verileri bağlanmış kontollerlede gösterebiliriz. Bunun için Single record in individual controls seçeneğini seçelim. Diğer seçenekleride olduğu gibi bırakalım.

![mk52_12.gif](images/mk52_12.gif)

Şekil 12. Son Adım. Formun görüntüsünün ayarlanması.

Böylece Data Form Wizard yardımıyla veritabanı uygulamamız için gerekli formu oluşturmuş olduk. Karşımıza aşağıdaki gibi bir Form çıkacaktır.

![mk52_13.gif](images/mk52_13.gif)

Şekil 13. Formun son hali.

Görüldüğü gibi tüm kontrolleri ile birlikte uygulamamız oluşturulmuştur. Sql sunucumuza Ole Db veri sağlayıcısı üzerinden erişmemizi sağlıyan OleDbConnection nesnemiz, tablolardaki verileri DataSet nesnesine yüklemek (Fill) ve bu tablolardaki değişiklikleri, veritabanına yansıtmak (update) için kullanılan OleDbDataAdapter nesneleri, tabloları ve tablolar arası ilişkiyi bellekte sakayıp veritabanından bağımsız olarak çalışmamızı sağlayan DataSet nesnemiz ve diğerleri... Hepsi sihirbazımız sayesinde kolayca oluşturulmuştur. Gelelim bu form ile neler yapabileceğimize.

Her şeyden önce uygulamamızı çalıştırdığımızda hiç bir kontrolde verilerin görünmediğine şahit olucaksınız. Önce, veritabanından ilgili tablolara ait verilerin DataSet üzerinden DataTable nesnelerine yüklenmesi gerekir. Bu Fill olarak tanımlanan bir işlemdir. Load butonu bu işlemi gerçekleştirir. Load başlıklı butonun koduna bakarasak;

```csharp
private void btnLoad_Click(object sender, System.EventArgs e)
{
     try
     {
          // Attempt to load the dataset.
          this.LoadDataSet();
     }
     catch (System.Exception eLoad)
     {
          // Add your error handling code here.
          // Display error message, if any.
        System.Windows.Forms.MessageBox.Show(eLoad.Message);
     }
     this.objdsSatislar_PositionChanged();
}
```

Görüldüğü gibi kod LoadDataSet adlı bir procedure'e yönlendirilir.

```csharp
public void LoadDataSet()
{
     // Create a new dataset to hold the records returned from the call to FillDataSet.
     // A temporary dataset is used because filling the existing dataset would
     // require the databindings to be rebound.
     Wizard.dsSatislar objDataSetTemp;
     objDataSetTemp = new Wizard.dsSatislar();
     try
     {
          // Attempt to fill the temporary dataset.
          this.FillDataSet(objDataSetTemp);
     }
     catch (System.Exception eFillDataSet)
     {
          // Add your error handling code here.
          throw eFillDataSet;
     }
     try
     {
          grdSiparisler.DataSource = null;
          // Empty the old records from the dataset.
          objdsSatislar.Clear();
          // Merge the records into the main dataset.
          objdsSatislar.Merge(objDataSetTemp);
          grdSiparisler.SetDataBinding(objdsSatislar, "Sepetler.drSepetlerToSiparisler");
     }
     catch (System.Exception eLoadMerge)
     {
          // Add your error handling code here.
          throw eLoadMerge;
     }
}
```

Bu metodda da yükleme işlemi için bir DataSet nesnesi oluşturulur veFillDataSet procedure'ü bu DataSet'i parametre alarak çağırılır. Bu kodlarda ise OleDbDataAdapter nesnelerinin Fill metodlarıa tablolara ait verileri, veritabanından alarak DataTable nesnelerine yüklerler. Bunu yaparken OleDbDataAdapter nesneleri Select sorgularını kullanır. Bu sorgular, program içinde oluşturulmuş SqlCommand nesnelerinde saklanmaktadır. Her bir tablo için bir OleDbDataAdapter, bu tabodaki verileri almak için çalıştıracağı bir Sql sorgusunu, bir SqlCommand nesnesinden alır.

```csharp
public void FillDataSet(Wizard.dsSatislar dataSet)
{
     // Turn off constraint checking before the dataset is filled.
     // This allows the adapters to fill the dataset without concern
     // for dependencies between the tables.
     dataSet.EnforceConstraints = false;
     try
     {
          // Open the connection.
          this.oleDbConnection1.Open();
          // Attempt to fill the dataset through the OleDbDataAdapter1.
          this.oleDbDataAdapter1.Fill(dataSet);
          this.oleDbDataAdapter2.Fill(dataSet);
     }
     catch (System.Exception fillException)
     {
          // Add your error handling code here.
          throw fillException;
     }
     finally
     {
          // Turn constraint checking back on.
          dataSet.EnforceConstraints = true;
          // Close the connection whether or not the exception was thrown.
          this.oleDbConnection1.Close();
     }
}
```

Uygulamanın diğer butonlarına ilişkin kodlarıda incelediğinizde herşeyin otomatik olarak sihirbaz tarafından uygulandığını göreceksiniz. Uygulamanın tüm kodlarını ekteki [DataForm1.cs](https://www.buraksenyurt.com/admin/app/editor/DataForm1.cs) dosyası içinden inceleyebilirsiniz. Şimdi uygulamamızı çalıştıralım ve görelim. Uygulamayı çalıştırdığınızda Form1'in ekrana geldiğini ve DataForm1 in görünmediğini göreceksiniz. Bu durumu düzeltmek için, projenin başlangıç formunu değiştirmemiz gerekiyor.Bunun için, Form1'in Main Procedure'ünün kodunu aşağıdaki gibi değiştirmemiz yeterlidir.

```csharp
static void Main()
{
     Application.Run(new DataForm1());
}
```

Şimdi uygulamamızı çalıştırabiliriz. Load başlıklı button kontrolüne tıkladığımızda tablo verilerinin ekrana geldiğini ve kontrollere yüklendiğini göreceksiniz. Yön kontrolleri ile Sepetler tablosunda gezinirken DataGrid kontrolündeki verilerinde değiştiğine dikkatinizi çekmek isterim.

![mk52_14.gif](images/mk52_14.gif)

Şekil 14.Uygulamanın çalışmasının sonucu.

Dilerseniz veriler üzerinde değişiklikler yapabilirsiniz. Ancak yaptığınız bu değişiklilker bellekteki DataSet kümesi üzerinde gerçekleşecektir. Bu değişiklikleri eğer, veritabanınada yazmak isterseniz Update başlıklı button kontrolüne tıklamanız gerekir. Bu durumda temel olarak, OleDbDataAdapter nesnesi, DataSet üzerindeki tüm değişiklikleri alıcak ve veritabanını Update metodu ile güncelleyecektir. Bu işlem için OleDbDataAdapter nesnesinin Update metodu, UpdateCommand özelliğine bakar. UpdateCommand özelliği, Update sorgusu içeren bir SqlCommand nesnesini işaret eder. Böylece güncelleme işlemi bu komut vasıtasıyla gerçekleşir. Bu işlemler yanında yeni satırlar ekleyebilir, var olanlarını silebilirsiniz. Görüldüğü gibi komple bir veritabanı uygulaması oldu. Bunun dışında uygulamamızın güçlü yönleride vardır. Örneğin yeni bir Siparis girin. Ancak Siparis numarasını aşağıdaki gibi var olmayan bir SepetID olarak belirleyin.

![mk52_15.gif](images/mk52_15.gif)

Şekil 15. Var olmayan bir ForeignKey girdik.

Bu durumda aşağıdaki uyarıyı alırsınız.

![mk52_16.gif](images/mk52_16.gif)

Şekil 16. ForeignKeyConstraint durumu.

Bu durumda Yes derseniz buradaki değer otomatik olarak 1000 yapılır. İşte bu olay DataRelation nesnemizin becerisidir. Değerli okurlarım uzun ama bir okadarda faydalı olduğuna inandığım bir makalemizin daha sonuna geldik. Hepinize mutlu günler dilerim.
