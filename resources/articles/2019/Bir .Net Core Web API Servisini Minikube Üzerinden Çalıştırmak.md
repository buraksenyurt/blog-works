---
title: "Bir .Net Core Web API Servisini Minikube Üzerinden Çalıştırmak"
pubDate: 2019-05-10 13:00:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - YAML
  - .NET
  - LINQ
  - Web API
  - HTTP
  - Docker
  - Kubernetes
  - Generics
  - GitHub
---

# Bir .Net Core Web API Servisini Minikube Üzerinden Çalıştırmak
![image.axd](images/image.axd)

Soğuk bir Şubat akşamı mıydı, dışarıda kar var mıydı, günün tam olarak hangi vakitleriydi tam olarak hatırlamıyorum ama github'a göre 24 numaralı örneğin son check-in işlemi 20 şubat Çarşamba günüydü.

[Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarına başladığımda hedefim sadece Cumartesi geceleri olmasına rağmen içten gelen bir motivasyon konulara haftanın herhangi bir gününde bakmamı sağlıyordu. Genelde ilgi çekici konular seçtiğimden başta belirlediğim standart çalışma takviminin dışına çıkmıştım. Bu hevesli motivasyon birinci fazın (41 bölümlük ilk faz olarak ifade edebilirim) tamamı boyunca süregeldi.

İç motivasyon kişisel gelişim açısından bence çok önemli bir sürükleyici. Doğruyu söylemek gerekirse onu bulduğumuz anda bir çalışmanın peşinden koşturmamıza da gerek kalmıyor. Kendiliğinden gelen disiplin bizi zaten o alana odaklıyor ve sonrasında fırtınada gemisini ustalıkla kullanırken yüksek sesle şarkılar söyleyen mutlu kaptan misali zaman prüssüzce akıyor.

İşte o Şubat günü bu motivasyonla tamamladığım bir çalışmam olmuş. Minikube konusunu incelemişim. Şimdi notların üstünden geçip derleme ve öğrendiklerimi gözden geçirme sırası. Haydi başlayalım.

Birden fazla konteyner'ın bir araya geldiği (Docker container'larını düşünelim), yönetilmeleri (Manegement), kolayca dağıtılmaları (Deployment) küçülerek veya büyüyerek ölçeklenebilmeleri (Scaling) gerektiği durumlarda orkestrasyon işi çoğunlukla Kubernetes (k8s) tarafından sağlanmakta. Kubernetes bir konteyner kümeleme (Clustering) aracı olarak Google tarafından Go dili ile yazılmış bir ürün. Ancak bazen deneme amaçlı olarak geliştirdiğimiz enstrümanları k8s kurulumuna ihtiyaç duymadan tek küme (Cluster) üzerinde çalışacak şekilde kurgulamak isteyebiliriz. Bu noktada minikube oldukça işimize yaramaktadır.

Benim 24 numaralı bu [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2024%20-%20WebAPI%20on%20Minikube) çalışmamdaki amacım Kubertenes'i WestWorld (Ubuntu 18.04, 64bit) üzerine kurmak yerine onu development ortamları için deneyimlememizi sağlayan Minikube'ü tanımaktı (Kubernetes'in tüm küme yapısının kurulumu çok da kolay değil. Üstelik sadece geliştirme noktasında onu denemek istersek bu maliyete girmeye gerek yok kanısındayım) Çalışma sırasında,.Net Core tabanlı bir Web API servisini içeren Docker konteynerının Minikube üzerinde koşturulması için gerekli işlemlere yer veriliyor.

> Minikube sayesinde Kubernetes ortamını local bir makinede deneme şansımız oluyor (Tabii belirli kısıtlar dahilinde) Minikube, VirtualBox veya muadili bir sanal makine içinde tek node olarak çalışan bir Kubernetes kümesi sunmakta. Dolayısıyla geliştirme katmanı için ideal bir ortam.

## İlk Kurulumlar

Linux ortamında Virtual Box isimli sanal makineye, Docker'a, Minikube ve onu komut satırından kontrol eden kubectl araçlarına ihtiyacımız var. WestWorld'de docker yüklü olduğu için diğerlerini kurarak ilerlemeye çalıştım. Virtual Box kurulumu için aşağıdaki terminal komutlarını kullanabiliriz.

```bash
sudo add-apt-repository multiverse
sudo apt-get update
sudo apt install virtualbox
```

Minukube kurulumunu içinse şöyle ilerleyebiliriz (Minikube ve bağımlılıklarının platforma göre farklı kurulumları için [şu adrese](https://kubernetes.io/docs/tasks/tools/install-minikube/) bakabilirsiniz)

```bash
curl -Lo minikube https://storage.googleapis.com/minikube/releases/v0.34.0/minikube-linux-amd64 && chmod +x minikube && sudo cp minikube /usr/local/bin/ && rm minikube
```

Kubernetes'i komut satırından yönetebilmek için kullanacağımız Kubectl aracını kurmak içinse aşağıdaki adımları takip edebiliriz.

```bash
sudo apt-get update && sudo apt-get install -y apt-transport-https
curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo apt-key add -
echo "deb https://apt.kubernetes.io/ kubernetes-xenial main" | sudo tee -a /etc/apt/sources.list.d/kubernetes.list
sudo apt-get update
sudo apt-get install -y kubectl
```

## Kurulum Sonrası Kontroller

Pek tabii kurulumlar sonrası bir sistem kontrolü yapmamızda yarar var. Docker, Virtual Box, Minikube ve kubectl gibi dört enstrümanın bir arada yaşayacağı bir geliştirme söz konusu. İlk olarak minikube servisini başlatmak lazım. Aşağıdaki terminal komutu ile bunu yapabiliriz.

```bash
minikube start
```

Hatta Minikube başarılı bir şekilde başladıktan sonra Virtual Box ortamından servis durumunu kontrol edebiliriz de. Aşağıdaki ekran görüntüsünde olduğu gibi minikube servisinin running modda görünmesi iyiye işarettir.

![image.axd](images/image.axd)

Çok doğal olarak servisi durdurma ve hatta silme ihtiyacımız da olabilir denemeler sırasında. Örneğin Minikube servisini durdurmak için,

```bash
minikube stop
```

silmek içinse,

```bash
minikube delete
```

komutlarından yararlanabiliriz.

## Örnek.Net Core Web API Uygulamasının Geliştirilmesi

Minikube orkestrasyonunda yönetmek istediğimiz servis veya servisler olması gerekiyor. Ben çalışma kapsamında geliştirme noktasında daha rahat hareket edebildiğim için.Net Core platformunu tercih ettim. Servis uygulaması Docker üzerinde koşacak. Oluşturmak için aşağıdaki terminal komutuyla ilerleyebiliriz.

```bash
dotnet new webapi -o InstaceAPI
```

InstanceAPI isimli servisimiz rastgele isim dönen bir metod sunmakta ki servisin ne iş yaptığı bu örnek özelinde çok önemli değil aslında. Ama pek tabii kodsal olarak yaptıklarımızı özetleyerek ilerlemekte yarar var. Ben varsayılan olarak gelen ValuesController sınıfını NamesController olarak aşağıdaki gibi değiştirdim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace InstanceAPI.Controllers
{
    [Route("api/random/[controller]")]
    [ApiController]
    public class NamesController : ControllerBase
    {
        List<string> nameList=new List<string>{
            "Senaida","Armand","Yi","Tyra","Maud",
            "Dominque","Jayme","Amira","Salome","Anisa",
            "Spencer","Angelyn","Pete","Hoa","Cherelle",
            "Lavonne","Gladys","Adrianne","Gussie","Delmar"
        };
        
        // HTTP Get talebine cevap veren metodumuz.
        // nameList koleksiyonundan rastgele bir isim döndürüyor
        [HttpGet]
        public ActionResult<string> Get()
        {
            Random randomizer=new Random();
            var number=randomizer.Next(0,21);
            return nameList[number];
        }
    }
}
```

Web API uygulamasını Dockerize etmek için aşina olduğunuz üzere Dockerfile dosyasına ihtiyacımız var ki onu da aşağıdaki şekilde kodlayabiliriz.

```bash
# Microsoft'ın dotnet sdk imajını aldık
FROM microsoft/dotnet:sdk AS build-env
# takip eden komutları çalıştıracağımız klasörü set ettik
WORKDIR /app

# Gerekli dotnet kopyalamalarını yaptırıp
# Restore ve publish işlemlerini gerçekleştiriyoruz
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Çalışma zamanı imajının oluşturulmasını istiyoruz
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
# Uygulamanın giriş noktasını belirtiyoruz
ENTRYPOINT [ "dotnet","InstanceAPI.dll" ]
```

Minikube içerisine neyin deploy edileceğini belirtmek için şimdilik deployment.yaml isimli dosyadan yararlanabiliriz. Bildirimlerden de görüldüğü üzere random-names-api-netcore isimli bir dağıtım söz konusu. Buna ait replica, label ve container gibi kubernetes odaklı bilgiler doküman içerisinde yazılmış durumda (Henüz bu konulara tam vakıf değilim. Çalışmaya devam)

```yml
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  name: random-names-api-netcore
spec:
  replicas: 3
  template:
    metadata:
      labels:
        app: random-names-api-netcore
    spec:
      containers:
        - name: random-names-api-netcore
          imagePullPolicy: Never
          image: random-names-api-netcore
          ports:
          - containerPort: 80
```

## Docker Hazırlıkları

Dockerfile dosyası tamamlandıktan sonra Web API uygulamasının dockerize edilmesine başlanabilir. Sonuçta k8s ya da örnekte ele aldığımız Minikube'ün ana görevi dockerize edilmiş örneklerin orkestrasyonunun sağlanması. Dockerize işlemi için build komutunu aşağıdaki gibi kullanmamız yeterli olacaktır.

```bash
docker build -t random-names-api-netcore .
```

## Minikube Deployment Hazırlıkları

Docker imajı hazır olduktan sonra artık minikube için gerekli dağıtım işlemine geçilebilir. Bu notkada kubectl komut satırı aracından yararlanmaktayız. kubectl, deployment.yaml dosyasının içeriğini kullanarak bir dağıtım işlemi icra edecektir. Aşağıdaki terminal komutları ile bu işlemleri gerçekleştirebiliriz. Bu işlemlere başlamadan önce minikube servisinin çalışır durumda olması gerektiğini hatırlatmak isterim.

```bash
kubectl create -f deployment.yaml
kubectl get deployments
kubectl get pods
```

create sonrasında kullanılan get komutları ile dağıtımı yapılan enstrümanı ve Podları görebiliriz (Pod = Aynı host üzerine dağıtımı yapılan bir veya daha fazla container olarak düşünülebilir ki senaryomuzda minikube için 3 pod söz konusudur) Lakin pod içeriklerine bakıldığında image durumlarının ErrImageNeverPull şeklinde kalmış olması gibi bir durum söz konusudur. En azından WestWorld'de böyle bir sorunla karşılaştığımı ifade edebilirim.

![image.axd](images/image.axd)

Sorun, minikube ile docker'ın birbirlerinden haberdar olmamalarından kaynaklanmaktaymış. Problemi aşmak için eval komutundan yararlanmak ve sonrasında docker imajını tekrar oluşturup minikube dağıtımını yeniden yapmak gerekiyor. Tabii önceki komutlar nedeniyle büyük ihtimalle sistemde duran dağıtımlar bulunacaktır. Önce onları silmek lazım. Aşağıaki ilk komutla dağıtım paketini bulup sonrasında silebiliriz.

```bash
kubectl get all
kubectl delete deployment.apps/random-names-api-netcore service/kubernetes
```

Temizlik tamamlandıktan sonra aşağıdaki terminak komutları ile ilerleyebiliriz. İlk komut docker'ı minikube örneği içerisinde çalıştırabilmek için gerekli yerel ortam parametrelerinin ayarlanmasını sağlıyor. Sonrasındaki komutlar tahmin edeceğiniz üzere docker imajının oluşturulması ve minikube ortamına dağıtım yapılması ile ilgili.

```bash
eval $(minikube docker-env)
docker build -t random-names-api-netcore .
kubectl create -f deployment.yaml
kubectl get deployments
kubectl get pods
```

![image.axd](images/image.axd)

## Çalışma Zamanı

Gelelim çalışma zamanına. Dockerize edilmiş servisimiz şu anda Minikube ortamında yaşıyor. Servisi dışarıya açmak için nodePort tipinden yararlanılmakta. Şu terminal komutları ile işlemlerimize devam edelim.

```bash
kubectl expose deployment random-names-api-netcore --type=NodePort
minikube service random-names-api-netcore --url
```

İlk komut ile dağıtımı yapılmış random-names-api-netcore isimli paket dışarıya açılmakta. İkinci terminal komutu ile servisin hangi adresten açıldığını öğrenebiliriz. Örneği denediğim zaman WestWorld'de 192.168.99.100 adresi ve 30046 nolu porttan hizmet verilmişti. Sonuç olarak bu adres bilgisinden servise erişip rastgele bir isim çekebiliriz.

> minikube aksini belirtmezsek 30000 ile 32767 port aralığını kullandırtmaktadır.

![image.axd](images/image.axd)

## 80 Numaralı Port

Daha yakın bir gerçek hayat senaryosu düşünüldüğünde ervisin 80 numaralı portan hizmet verebilecek şekilde çalıştırılması önemlidir. Bunu sağlamak minikube tarafında bir servis kurgusuna ihtiyacımız var. Servisleri bir cluster üzerinde çalışan pod grupları olarak düşünebiliriz. Dolayısıyla birden fazla pod'un tek bir servismiş gibi dışarıya sunulması söz konusudur. 80 numaralı port içinde buna benzer bir hazırlığa ihtiyacımız var. Bunun için uygulamaya services.yaml isimli bir dosyanın eklenmesi gerekiyor. Bu dosyada NodePort değeri 80 olarak belirtilmekte. Dosya içeriğimiz aşağıdaki gibidir (Pod ve Service konusu ile ilgili olarak daha fazla bilgi için [şu yazı](https://github.com/chrislusf/seaweedfs/wiki/Deployment-to-Kubernetes-and-Minikube)ya bir göz atabilirsiniz)

```text
apiVersion: v1
kind: Service
metadata:
  name: random-names-api-netcore
  labels:
    app: random-names-api-netcore
spec:
  type: NodePort
  ports:
  - port: 80
    targetPort: 80
    nodePort: 80
    protocol: TCP
  selector:
    app: random-names-api-netcore
```

Sonrasında sırasıyla dağıtımı yapılan varlıklar silinir (Belki de buna gerek yoktur, araştırmak lazım) minikube 80 ile 30000 aralığını baz alacak şekilde yeniden başlatılır ve servis tekrardan oluşturulur. Bu kez dikkat edileceği üzere kubectl create komutu deployment.yaml yerine services.yaml dosyasını kullanmaktadır.

```bash
kubectl delete service random-names-api-netcore
kubectl delete deployment random-names-api-netcore
minikube start --extra-config=apiserver.service-node-port-range=80-30000
kubectl create -f services.yaml
```

![image.axd](images/image.axd)

> İşlemleri başarılı bir şekilde sonlandırdık diyebiliriz. Evden çıkmadan önce minikube stop komutunu vermek yararlı olabilir.

## Ben Neler Öğrendim

Bu çalışmanın yarattığı eğlenceli dakikaları geride bırakırken aşağıdaki maddelerde yazılanları öğrendiğimi not olarak düşmüşüm. Bir kaç zaman sonra bu notlara baktığımda yeniden düşünüyorum. Gerçekten ne kadarı aklımda kalmış ne kadarını doğru hatırlıyorum... Sonuçta unuttuklarım da olmuş ve bunları yeniden ele almak Saturday-Night-Works çalışmasına başlamamın ne kadar isabetli bir karar olduğunu kendi adıma ispat ediyor.

- Kubernetes kurulumları ile uğraşmak yerine development amaçlı olarak Minikube kullanılmasını
- Temel kubectl komutlarını
- Pod ve Service kavramlarının ne anlama geldiğini
- .Net Core Web API uygulamasının basitçe Dockerize edilmesini
- Minkube ortamının sunduğu port numarasının 80e nasıl çekileceğini
- Dockerfile, deployment.yaml ve services.yaml içeriklerindeki kavramların ne anlama geldiklerini

Böylece geldik bir cumartesi gecesi macerasının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
