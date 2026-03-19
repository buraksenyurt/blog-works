---
layout: post
title: "Task Parallel Library(TPL) - Detached Tasks [Beta 2]"
date: 2009-11-12 06:00:00 +0300
description: "Bir önceki yazımızda Task Parallel Library tarafında .Net Framework 4.0 Beta 2 tabanlı olarak iptal işlemleri(Task Cancellation) için yapılan değişikliklere değinmeye çalışmıştık. TPL tarafında yapılan değişikliklerden birisi de iç içe çalışan Task' ler arasındaki Parent - Child ilişkiye yönelik olarak yapılmıştır. Aslında basit bir davranış değişikliği olduğunu söyleyebiliriz. Konuyu daha net kavramak amacıyla aşağıdaki örnek kod parçasını göz önüne alalım."
categories:
  - tpl
tags:
  - tpl
  - csharp
  - dotnet
  - task-parallel-library
  - threading
---
# Task Parallel Library(TPL) - Detached Tasks [Beta 2]
Merhaba Arkadaşlar,

Bir önceki yazımızda Task Parallel Library tarafında.Net Framework 4.0 Beta 2 tabanlı olarak iptal işlemleri (Task Cancellation) için yapılan değişikliklere değinmeye çalışmıştık. TPL tarafında yapılan değişikliklerden birisi de iç içe çalışan Task'ler arasındaki Parent - Child ilişkiye yönelik olarak yapılmıştır. Aslında basit bir davranış değişikliği olduğunu söyleyebiliriz. Konuyu daha net kavramak amacıyla aşağıdaki örnek kod parçasını göz önüne alalım.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DetachedTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 1 başlangıç zamanı {0}",DateTime.Now.ToLongTimeString());
                Task task2 = Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Task 2 başlangıç zamanı {0}", DateTime.Now.ToLongTimeString());
                        Thread.Sleep(6000);
                    }
                );
                Thread.Sleep(3000);
            }
            );

            task1.Wait();
            Console.WriteLine("Program sonu :  {0}",DateTime.Now.ToLongTimeString());
        }
    }
}
```

Öncelikli olarak kodumuzda neler yaptığımıza bir bakalım. Örnekte iki ayrı Task nesnesinin kullanıldığı görülmektedir. task2 isimli nesne örneği, task1 içerisinde üretilmekte ve kullanılmaktadır. Buna göre task1' in, task2' nin parent'ı olması gerektiğini düşünebiliriz. Aslında önemli olan nokta task1 üzerinden yapılan Wait çağrısı sonucu programın nasıl davranacağıdır. Bu noktada task1.Wait () çağrısının Beta 1 sürümünde farklı değerlendirildiğini belirtmemiz gerekiyor. Şöyle ki;

Beta 1 sürümüne göre task2 otomatik olarak task1' e bağımlı hale getirilmekteydi (Attached Task). Yani task1 üzerinden yapılan Wait çağrısı sonucu task1' in tamamlanması beklenirken, içeride çalışmakta olan task2' ninde çalışmasını tamamlanması gerekmekteydi. Bu varsayılan davranış biçimiydi. Ancak bazı durumlarda task1 beklenirken, içerisinde yer alan task2' nin beklemesi istenmeyebilir. Bir başka deyişle Wait çağrısı sonrasına geçilmesi için task1' içerisinde alt Task'ler tarafından yapılan işlemler dışında kalanların bitirilmesinin yeterli olması istenebilir. Beta 1' de bu durumu gerçeklemek için TaskCreationOptions.DetachedFromParent enum sabiti değerinde yararlanılmaktaydı (ki Beta 2' de kaldırıldı). Ancak Beta 2 sürümünde durum değiştirildi.

Beta 2 sürümüne göre varsayılan olarak alt Task nesne örnekleri içerisinde yer aldıkları üst Task'lere bağlı değildir (Varsayılan olarak Detached). Açıkçası, varsayılan olarak Task'lerin Detached olarak tesis edildiklerini ifade edebiliriz. Buna göre yukarıdaki kodun çalışma zamanı çıktısı Beta 2 sürümü için aşağıda görüldüğü gibi olacaktır.

![blg99_FirstRunLast.gif](/assets/images/2009/blg99_FirstRunLast.gif)

Dikkat edileceği üzere task1 ile aynı zaman dilimi içerisinde task2 başlatılmış ancak task2 nin tamamlanması beklenmeden task1' deki işlemler bittiği için program sonuna gelinmiştir. Nitekim task2 varsayılan olarak task1' e bağlanmadığından (Detached) task1.Wait çağrısı gerçekten sadece task1' in işleyişinin tamamlanması ile ilgilenmiştir. Peki task2' nin task1' e bağlanması için ne yapılmalıdır? Bu amaçla task2 nesne örneğinin üretildiği yerde TaskCreationOptions.AttachedToParent enum sabiti değerinin kullanılması gerekmektedir. Dolayısıyla kodu aşağıdaki gibi güncellememiz yeterli olacaktır.

```csharp
Task task1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 1 başlangıç zamanı {0}",DateTime.Now.ToLongTimeString());
                Task task2 = Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Task 2 başlangıç zamanı {0}", DateTime.Now.ToLongTimeString());
                        Thread.Sleep(6000);
                    },TaskCreationOptions.AttachedToParent
                );
                Thread.Sleep(3000);
            }
            );
```

Kodun bu şekilde çalıştırılması sonucu aşağıdaki ekran çıktısı ile karşılaşılacaktır.

![blg99_SecondRun.gif](/assets/images/2009/blg99_SecondRun.gif)

AttachedToParent değeri nedeni ile task2, task1' e bağlı hale getirilmiştir. Yani task1, task2' nin parent Task'i olarak belirlenmiştir. Bu durumda task1.Wait çağrısının yapıldığı noktada Attach edilmiş tüm Task referansları değerlendirileceğinden task1' inde tamamlanması beklenilmiştir. Bu durum her iki çalışmadaki Program Sonu sürelerinden anlaşılabilmektedir. Nitekim ilk çalışmada task2 içerisindeki 6 saniyelik Sleep çağrısı hesaba katılmazken, ikinci örnekte katılmıştır. Task Parallel Library ile ilişkili olarak Beta 2' de gelen diğer değişiklikleri ele aldığımız başka bir yazımızda görüşmek dileğiyle, hepinize mutlu günler dilerim.