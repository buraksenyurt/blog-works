---
title: "Net Data Providers(Veri Sağlayıcıları)"
pubDate: 2004-01-22 08:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - .NET
  - SQL Server
  - Oracle
  - Performance
---

# Net Data Providers(Veri Sağlayıcıları)
Değerli Okurlarım, Merhabalar.

Bugünkü makalemiz ile, ADO.NET ' te yer alan veri sağlayıcılarını inceleyeceğiz. Bildiğiniz gibi hepimiz uygulamalarımızda yoğun bir şekilde veri kaynaklarını kullanmaktayız. Normalde sistemimizde, bu veri kaynaklarına erişmek için kullanılan sistem sürücüleri vardır. Bu sürücüler, sistemimize dll kütüphaneleri olarak yüklenirler ve kendilerini sisteme kayıt ederler (register). Bu noktadan itibaren bu veri sürücülerinin içerdiği fonksiyonları kullanarak veritabanları üzerinde istediğimiz işlemleri gerçekleştirebiliriz. Kısaca, bu veri sürücüleri uygulamalarımız ile, veritabanı arasındaki iletişimi sağlarlar. Sistemizide yüklü olan programlara göre pek çok veri sürücüsüne sahip olabiliriz. Örneğin ODBC sürücüleri, SQL sürücüleri, Ole Db Jet sürücüleri ve bazıları.

ADO.NET ile veritabanı uygulamaları geliştirirken, bu sürücüler üzerinden veritabanlarına erişim sağlarız. Bu sebeple.Net Framework 'te her bir veri sürücüsü için geliştirilmiş veri sağlayıcıları (data providers) vardır. Bu veri sağlayıcılarının görevi, uygulamalarımız ile veri sürücülerini bağlamak ve veri sürücülerindeki ilgili kütüphane fonksiyonlarını çalıştırarak veriler üzerinde işlem yapabilmemizi sağlamaktır..Net Framework'ün 1.1 sürümü aşağıdaki listede yer alan veri sağlayıcıları ile birlikte gelmektedir..Net Framework'ün ilk sürümlerinde sadece Sql ve Ole Db veri sağlayıcıları varsayılan olarak yer almaktadır. Ancak 1.1 sürümü ile birlikte bu veri sağlayıcılarına, Oracle ve ODBC veri sağlayıcılarıda eklenmiştir.

.Net Framework Veri Sağlayıcıları

Data Provider For SQL Server

Data Provider For OLE DB

Data Provider For ODBC

Data Provider For Oracle

Tablo 1:.NET Veri Sağlayıcıları

Şimdi dilerseniz, bu veri sağlayıcıları kısaca incelemeye çalışalım.

SQL veri sağlayıcısına ait tüm üyeler, System.Data.SQLClient isim uzayında yer almaktadır. SQL veri sağlayıcısının en önemli özelliği, sql motoruna direkt sql api'si üzerinden erişim sağlayabilmesidir. Bu özellik ona diğer veri sağlayıcılarına göre daha yüksek performans kazandırır. Nitekim sql veri sağlayıcısı, sql server'a doğrudan ulaşmak için kendi iletişim protokolü olan TDS (Tabular Data Stream)'yi kullanmaktadır. Elbette bu özelliği ile, örneğin SqlDataReader nesnesinin kullanıldığı veri okuma yöntemlerinde, ole db veri kaynağına göre çok daha hızlı ve verimlidir. Nitekim aynı sql veri kaynaklarına ole db veri sağlayıcısı ilede erişmemiz mümkündür. Ama belirttiğimiz gibi performans ve verimlilik bu iki veri kaynağı için oldukça farklıdır.

![mk44_1.gif](images/mk44_1.gif)

Şekil 1. Sql Veri Sağlayıcımız.

Sql veri sağlayıcısı, Sql Server'ın 7 ve daha üstü versiyonlarını desteklemektedir. Bu nedenle 6.5 versiyonu ve daha öncesi için, Ole Db veri sağlayıcısını kullanmak zorundayız. Diğer yandan Sql veri sağlayıcısı MDAC (Microsoft Data Access Component)'ın 2.6 veya üstü sürümünün sistemimizde kurulu olmasını gerektirmektedir. Sql veri sağlayıcısı, sql server'ın 7.0 ve sonraki sürümlerinde özellikle çok katlı uygulamalarda yüksek verim ve performans sağlar.

Ole Db veri sağlayıcısı, Ole Db desteği veren tüm veri sürücüleri ile ilişki kurabilmektedir. Bunu yaparken, Ole Db Com nesnelerini kullanır. Aşağıdaki şekilde görüldüğü gibi, uygulamamızda ole db veri sağlayıcısı kullanarak, bir oledb veri kaynağına erişmek oldukça maliyetlidir. Bunun yanında ole db'yi destekleyen çok çeşitli veri kaynağı sürücülerinin olması ole db nin ürün yelpazesini genişliğini gösterir.

![mk44_2.gif](images/mk44_2.gif)

Şekil 2. Ole Db Veri Sağlayıcısı

Ole Db veri sağlayıcısı Ole Db desteği veren her türlü veri sürücüsü ile çalışabilir. Aşağıda ole db veri sağlayıcısı ile kullanılabilen örnek Ole Db veri sürücüleri listelenmiştir.

![mk44_3.gif](images/mk44_3.gif)

Ole Db veri sağlayıcısının.net framework üyeleri, System.Data.OleDb isim uzayında yer alır. Çoğunlulkla bu veri sağlayıcısını Access tablolarına erişmek için uygulamalarımızda kullanmaktayız. Bununla bilrikte Paradox, dBASE, Excel, FoxPro,Oracle 7.3,Oracle8 gibi veri tablolarınada erişebiliriz. Diğer yandan Oracle sürücülerine ve ODBC sürücülerinede erişebiliriz. Ancak elbetteki, çok katlı uygulamalarda, sql veri sağlayıcısını veya oracle veri sağlayıcısını tercih etmemiz daha doğru olucaktır. Diğer yandan ole db veri sağlayıcısı, com servsileri ile veri sürücülerine eriştiği için, özellikle sql veri sağlayıcısına göre çok daha düşük bir performans sergiler. Ole Db veri kaynakları ile çalışan ole db veri sağlayıcısının, özellikle sql server'ın 6.5 ve önceki sürümlerinin kullanıldığı tek katlı ve çok katlı uygulamalarda kullanılması tercih edilir. Bununla birlikte, access tabloları ile çalışırken, çok katlı mimarilerin, bu veri tabloları üzerinden ole db sağlayıcıları ile oluşturulması microsoft otoriterlerince tavsiye edilmemektedir.

ODBC veri sağlayıcısı, Ole Db veri sağlayıcısı gibi, ODBC desteği veren sürücüler ile, ODBC Servis Component'lerini kullanarak iletişim kurar.

![mk44_4.gif](images/mk44_4.gif)

Şekil 3. ODBC Veri Sağlayıcısı

ODBC veri sağlayıcısı ile ilgili üyeler,.net framework içinde, System.Data.Odbc isim uzayında yer almaktadır. Aslında bu veri sağlayıcı,.net framework'ün 1.0 versiyounda yer almamaktaydı. Ancak 1.1 verisyonu ile birlikte ADO.NET ' teki yerini almıştır. ODBC sürücüsü yardımıyla,sql server'a, access tablolarına ve odbc'yi destekleyen veri srücülerine erişebiliriz. ODBC veri sağlayıcısı, odbc veri kaynakları üzerinden yapılan tek katlı (single-tier) ve orta katlı (middle-tier) mimarilerinde kullanılabilir.

Oracle servis sağlayıcısı,.net framework'ün System.Data.OracleClient isim uzayında yer alan üyelerden oluşur. Oracle servis sağlayıcısı, oracle veri kaynaklarına erişebilmek için, sql veri sağlayıcısı gibi kendi iletişim protokünü içeren Oracle Client Connectivity'yi kullanır. Oracle veri sağlayıcısının.net'e yerleştirilmesindeki temel amaç, oracle veri tabanlarına ole db veri sağlayıcısı ile ole db üzerinden değil, doğrudan erişilebilmesini sağlamaktır. Bu sayede oracle veri kaynağı ile oluşturulan etkileşimde en iyi performansın elde edilmesi sağlanmıştır. Zaten bu yönü ilede oracle veri sağlayıcısı, sql veri sağlayıcısına benzer bir yapıdadır. Doğal olarak, oracle veri kaynakları üzerinde gerçekleştirilen, çok katlı ve tek katlı mimarilerde yüksek performans sergilemektedir.

Bu makalemizde.net veri sağlayıcılarına kısaca değinmeye çalıştık. İlerliyen makalelerimiz ile birlikte ado.net'in tüm kavramlarını inclemeye çalışacağım. Bir sonraki makalemde ole db veri sağlayıcısı üyelerinden olan, OleDbConnection nesnesini incelemeye çalışacağım. Hepinize mutlu günler ve iyi çalışmalar dilerim.
