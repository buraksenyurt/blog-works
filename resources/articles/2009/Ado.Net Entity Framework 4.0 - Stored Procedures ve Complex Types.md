---
title: "Ado.Net Entity Framework 4.0 - Stored Procedures ve Complex Types"
pubDate: 2009-11-15 23:55:00
categories:
  - Entity Framework
tags:
  - Entity Framework
  - C#
  - .NET
  - ADO.NET
  - LINQ
  - HTTP
  - Visual Studio
---

# Ado.Net Entity Framework 4.0 - Stored Procedures ve Complex Types
Merhaba Arkadaşlar,

Ado.Net Entity Framework 4.0 ile birlikte gelecek/gelmekte olan yeniliklerden birisi de, Stored Procedure'lerin dönüş tipi ile alakalıdır. Henüz tam olarak bitirilememiş olan bu özellik şu anki haliyle bir Stored Procedure'den geriye karmaşık bir tipinin (Complex Type) döndürülebilmesine izin vermektedir. Bunun için Designer tarafında destek sunulmaktadır. Aslında önceki Ado.Net Entity Framework sürümünde bir Stored Procedure'ün Entity modeli içerisine eklenmesi sonrasında dönüş kümesinin Scalars veya Entities olarak kullanılması sağlanabilmekteydi. Ancak bir Stored Procedure çıktısının Complex Type bazlı olaraktan kod tarafında ele alınamayışı da de önemli bir eksiklikti. Bakalım 4.0 versiyonunda bu eksikliği gidermek adına neler yapılmış. Yazımızın ilerleyen kısımlarında bu özeliği anlamaya çalışıyor olacağız.

İlk olarak kendimize kobay bir Stored Procedure seçelim

![Wink](images/smiley-wink.gif)

Bu amaçla Adventure Works veritabanında yer alan uspGetManagerEmployees Stored Procedure'ünü kullanabiliriz. Bu procedure parametre olarak ManagerID isimli int tipinden bir dğer almakta ve aşağıdaki örnek çıktıyı üretmektedir.

![blg101_SpExec.gif](images/blg101_SpExec.gif)

Pekala Stored Procedure'ümüzün çıktısı, RecursionLevel,ManagerID,ManagerFirstName, ManagerLastName, EmployeeID, FirstName ve LastName alanlarının karşılıklarını özellik (Property) olarak içeren sınıfa ait nesne örneklerinden oluşan bir nesne kümesi ile ifade edilebilir. Bunu gerçekleştirmek için Visual Studio 2010 Ultimate Beta 2 sürümünde oluşturacağımız Console uygulamamıza Adventure Works veritabanı için yeni bir Ado.Net Entity Data Model öğesi ekleyip sadece uspGetManagerEmployees SP'sini seçtiğimizi düşünelim.

![blg101_AddSp.gif](images/blg101_AddSp.gif)

Şimdi adım adım ilerleyerek Complex Type üretimini gerçekleştireceğiz.

İlk olarak Model Browser ve AdventureWorksModel.Store kısmından ilgili Stored Procedure adına sağ tıklanıp Add Function Import seçimi yapılmalıdır.
Seçimin ardından gelen ekranda Get Column Information düğmesine basıldığı takdirde ilgili Stored Procedure'den dönen sonuç kümesi için uygun olan kolon bilgilerinin getirildiği görülür. Burada hem EDM Type hemde SQL Type bilgileri yer almaktadır.
Sonraki adımda ise Create New Complex Type düğmesine tıklanarak Stored Procedure'ün sonuç kümesi için gerekli sınıfın üretilmesi sağlanır.
İstenirse üretilen tip adı Complex kutucuğundan değiştirilir (Biz örneğimizde tip adını ManagerEmployees olarak yeniledik)
Ok tuşuna basılarak işlem tamamlanır.

Aşağıdaki şekilde bahsetmiş olduğumuz adımlar görsel olarak özetlenmektedir.

![blg101_AddFunctionImport.gif](images/blg101_AddFunctionImport.gif)

Şimdi arka planda neler olduğuna bir bakalım. İlk olarak Entity Data Model tarafında söz konusu Stored Procedure için bir Return Type ve Mapping Details kısmında gerekli kolon-özellik eşleştirmelerinin yapıldığı görülür.

![blg101_FirstTrace.gif](images/blg101_FirstTrace.gif)

![blg101_MappingDetails.gif](images/blg101_MappingDetails.gif)

Buna ek olarak Return Type için kod tarafında ManagerEmployees isimli ComplexObject türevli bir sınıfın üretildiği gözlemlenir. Class Diagram görüntüsünde bu durum açık bir şekilde izlenebilmektedir.

![blg101_ClassDiagram.gif](images/blg101_ClassDiagram.gif)

Çok doğal olarak söz konusu Stored Procedure'ün kod içerisinde kullanılabilmesi için gerekli özelliğin de AdventureWorksEntities Context tipi içerisine eklendiği görülebilir.

![blg101_Property.gif](images/blg101_Property.gif)

Dikkat edileceği üzere uspGetManagerEmployees isimli özelliğin (Property) dönüşü ObjectResult tipindendir. Bu tip IEnumerable ve IEnumerable arayüzlerini (Interface) implemente ettiğinden LINQ sorgularında kaynak veri olarak kullanılabilir.

Dilerseniz birde söz konusu Stored Procedure'e ait çıktıyı örnek kod parçasında değerlendirmeye çalışalım. Bunun için aşağıdaki gibi basit bir kodlama yapmamız yeterli olacaktır.

```csharp
using System;
using System.Data.Objects;
using System.Linq;

namespace SPAndComplexType
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AdventureWorksEntities context = new AdventureWorksEntities())
            {
                ObjectResult<ManagerEmployees> resultSet=context.uspGetManagerEmployees(16);

                var subResult = from me in resultSet
                                where me.FirstName[0] == 'M'
                                select me;

                foreach (var managerEmployee in subResult)
                {
                    Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}",
                        managerEmployee.EmployeeID.ToString(),
                        managerEmployee.FirstName,
                        managerEmployee.LastName,
                        managerEmployee.ManagerFirstName,
                        managerEmployee.ManagerID,
                        managerEmployee.ManagerLastName,
                        managerEmployee.RecursionLevel
                        );
                }
            }
        }
    }
}
```

Örnek kod parçasında uspGetManagerEmployess özelliği çağırılmış ve ManagerID değeri 16 olan Employee listesinin getirilmesi sağlanmıştır. Bu işlemin arkasından da elde edilen sonuç kümesi üzerinden basit bir LINQ sorgusu çalıştırılmış ve FirstName alanının ilk harfi M olanların listelenmesi sağlanmıştır. Hemen bir hatırlatma yapalım; SQL tarafındaki uspGetManagerEmployees Stored Procedure'ünün çalıştırılması context nesne örneği üzerinden uspGetManagerEmployess özelliğinin çağırılması ile birlikte gerçekleşmektedir. Son olarak uygulamayı çalıştırdığımızda aşağıdaki çıktıyı elde ettiğimizi görürüz.

![blg101_Runtime.gif](images/blg101_Runtime.gif)

Böylece geldik bir yazımızın daha sonuna. Ado.Net Entity Framework 4.0 ile ilişkili yenilikleri öğrendikçe sizlerede aktarmaya çalışıyor olacağım. Bu konuda ilk elden bilgi almak isteyen arkadaşlarımızın[Ado.Net Team Blog'](http://blogs.msdn.com/adonet/default.aspx)unu mutlaka takip etmesini öneririm. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SPAndComplexType.rar (36,40 kb)](assets/SPAndComplexType.rar)
