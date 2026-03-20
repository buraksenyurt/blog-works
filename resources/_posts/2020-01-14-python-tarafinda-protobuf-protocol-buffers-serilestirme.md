---
layout: post
title: "Python Tarafında ProtoBuf (Protocol Buffers) Serileştirme"
date: 2020-01-14 21:00:00 +0300
categories:
  - python
tags:
  - python
  - bash
  - javascript
  - xml
  - rest
  - json
  - http
  - grpc
  - performance
  - serialization
  - github
---
Protocol Buffer, Google'ın yapısal verileri (structured data) serileştirmek için geliştirdiği bir protokol (Hatta gRPC ile sıklıkla anılır) Onu XML (eXtensible Markup Language) benzeri bir veri tanımlama formatı olarak düşünebiliriz ama çok daha az yer tutar ve serileştirme süresi çift yönlü olarak daha kısadır. Şu sıkça gördüğümüz proto uzantılı dosyaların ana fikridir.

![protobuf2.png](/assets/images/2020/protobuf2.png)

Nam-ı diğer ProtoBuf örneğin ağ üzerinden gönderilecek verinin tanımını yapan bir sözleşmedir. Sözleşmenin ana unsuru ise mesajdır. Dosya içeriğine bakınca anlaşılması kolay bir mevzu iken tek başına bir anlamı yoktur, nitekim protoc isimli derleyiciden de geçmesi gerekir. Bu derleme sonucu insan gözüyle pek de okunamayan binary bir versiyon ortaya çıkar. Bunu ağ üstünden gönderebilir bir yerlerde veri olarak saklayabilir ya da ille de okumak istersek JSON, XML gibi formatlara dönüştürebiliriz.

Mikroservisler denince akla gelen REST, WebSockets ve GraphQL iyidir hoştur ama yüksek hız gerektiren durumlar varsa low-level RPCs (Remote Procedure Calls) önem kazanır. Lakin uygulanması çok kolay olmadığından bizleri over-engineering anti-pattern'ine de sürükleyebilir. Bu arada ProtoBuf'ın JSON serileştirmeli örneklerden 6 kat daha performanslı olduğu da iddia edilmiş. Detaylı bilgi için [buraya](https://auth0.com/blog/beating-json-performance-with-protobuf/) bakınız.

Benim bu örnekteki amacım ProtoBuf serileştirme işlemini Heimdall (Ubuntu 20.04) üzerinde Python kodları ile kullanabilmekti. Bir veri deseni oluşturmak ve bunu çift yönlü olarak serileştirmek kafi olacaktı. ProtoBuf'ın uygulandığı tüm dillerde esas itibariyle benzer altyapı söz konusu. Olay bir proto sözleşmesinin yazılıp ilgili dil için protoc derleyicisi ile hazırlanması ve kullanımından ibaret.

## Hazırlıklar, Kodlama ve Çalışma Zamanı

Tabii heimdall üzerinde protoc derleyicisi yüklü değildi. Yani yazıyı okuyan siz değerli okurun sisteminde de bu derleyici yüklü olmayabilir. Bu nedenle ben [şu adresteki](http://google.github.io/proto-lens/installing-protoc.html) kurulum talimatlarını izledim.

```bash
PROTOC_ZIP=protoc-3.7.1-linux-x86_64.zip
curl -OL https://github.com/protocolbuffers/protobuf/releases/download/v3.7.1/$PROTOC_ZIP
sudo unzip -o $PROTOC_ZIP -d /usr/local bin/protoc
sudo unzip -o $PROTOC_ZIP -d /usr/local 'include/*'
rm -f $PROTOC_ZIP

protoc --version
```

Örnekte birde protobuf dosyamız mevcut elbette. Yani ağ üzerinden koşturacağımız veri desenimiz. Ben örnek olarak bir görev bilgisini tasarlamaya çalıştım. Görevin başlığı, açıklaması, durumu gibi bilgilerin yer aldığı bir veri modeli...

```javascript
syntax="proto3"; // Kullandığımız ProtoBuf versiyonu

package company;

// görev durumunu tutacağımız enum sabiti
enum State{
    PLANNED=0;
    IN_PROGRESS=1;
    DONE=2;
}

// ana tipimiz Job
// içinde n sayıda Task içerebilir ki o da bir sözleşme tipidir
// niteliklere atanan sayılar binary encode edilen çıktıdaki alanları işaret eden benzersiz sayılardır. 
// Bir nevi eşleştirme veya yer bulma tekniği.
message Job{    
    int32 job_id=1;
    string title=2;

    // İşteki görevleri tanımlayan tipimiz
    message Task{
        int32 task_id=1;
        string title=2;
        string description=3;
        State state=4; //enum sabiti tipinden
    }

    repeated Task tasks=3; //Task tipinin tekrarlanabileceğini belirtiyor
}
```

Başta da belittiğimiz üzere yazılan bu proto dosyasının pyhton çalışma zamanı ortamında kullanılabilmesi için bir derleme işleminden geçmesi gerekiyor. Bunu aşağıdaki terminal komutları yardımıyla gerçekleştirebiliriz. Sonuç olarak jobs_pb2.py isimli bir dosya üretilecektir. Bu paket izleyen app.py içerisinde kullanılmaktadır. Yorum satırlarını takip edelim;)

```bash
protoc -I=. --python_out=. ./jobs.proto
```

Gelelim protobuf içeriğini kullanarak örnek serileştirme ve ters-serileştirme işlemlerini yapan Python kodlarımızın yer aldığı app nesnesine.

```bash
# encoding:utf-8

import jobs_pb2 as Job  # Protoc dosyasından üretilen paketi ekledik

# Yeni bir Job nesnesi tanımlayıp içine birkaç Task ekliyoruz
job = Job.Job()
job.job_id = 1802234
job.title = "Şube personeli fatura onay sürecine olarak belge yükleyebileceğim bir ekran istiyorum."

task1 = job.tasks.add()
task1.task_id = 1
task1.title = "Ekran Tasarımı"
task1.description = "Fatura yükleme ekranının tasarımı"
task1.state = Job.State.Value("IN_PROGRESS")

task2 = job.tasks.add()
task2.task_id = 2
task2.title = "Fatura Şema Tasarımı"
task2.description = "Fatura içeriği için gerekli protoc içeriğinin tasarımı"
task2.state = Job.State.Value("PLANNED")

# print(job)  # Ekrana çıktıyı basalım

# Job içeriğini jobs.data isimli dosya içine serileştiriyoruz
with open("./jobs.data", "wb") as file:
    file.write(job.SerializeToString())

print("İçerik Serileştirildi...")
print(job.SerializeToString())
print()

incoming_job = Job.Job()  # yeni bir job nesnesi oluşturalım
# jobs.data içeriğini okuyup incoming_job içerisine ters serileştirelim
with open("./jobs.data", "rb") as file:
    incoming_job.ParseFromString(file.read())

print("Şimdi Dosyadan Ters Serileştirildi")
print(incoming_job)
```

Yazdığımız örneği aşağıdaki terminal komutu ile çalıştırabilir ve sonuçları irdeleyebiliriz.

```bash
python3 app.py
```

![skynet_10_Screenshot_1.png](/assets/images/2020/skynet_10_Screenshot_1.png)

Esasında örnekte protobuf olarak tasarlanmış bir nesnenin python ile serileştirme işlemlerinde nasıl kullanıldığını incelemiş olduk. Gerçek hayat örneği için bu kurguyu bir servisin arkasına taşımak çok daha doğru olur. Örneğin Flask ile bir servis geliştirip istemci ile haberleşilen veriler için JSON yerine protobuf kullanımı örneklenebilir. Bu pekala sizin için güzel bir ödev de olabilir:) O halde ne duruyorsunuz!? Haydi başlayın.

Örneğin tüm kodlarına ve ilk notlarına [Skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2010%20-%20Protocol%20Buffers%20with%20Python/src)ndan erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
