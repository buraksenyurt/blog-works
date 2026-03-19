---
title: "GoLang - Google ProtoBuf Kullanımı"
pubDate: 2017-07-22 21:03:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - XML
  - JSON
  - Go
  - Python
  - Java
  - JavaScript
  - Performance
  - Serialization
  - Pointers
  - GitHub
---

# GoLang - Google ProtoBuf Kullanımı
![protogopher_1.gif](images/protogopher_1.gif)

Merhaba Arkadaşlar,

Uygulama verilerini kullandığımız dile göre çeşitli şekillerde ifade edebiliriz. Eğer nesne yönelimli bir dil kullanıyorsak buradaki başrol oyuncumuz sınıflardır. Verinin nesnel olarak ifade edilişinde rol olan sınıf ve benzeri tipler, çalışma zamanında taşıdıkları içerikleri ile sürekli hareket halindedir. Bu hareket uygulamanın kendi alanında olabileceği gibi farklı programlar arasında da gerçekleşebilir. Veri, ağ üzerinde de hareket edebilir. Verinin bu şekilde dolaşımı sırasında belirli kriterlere göre serileştirilmesi de gerekebilir. Bu noktada karşımıza platform bağımsızlık, okunabilirlik, genişletilebilirlik, versiyonlama ve performans gibi kriterler çıkar.

Bir fabrikanın üretim hattına ait bilgileri barındıran bir veri modelini tasarladığınızı ve çalışma zamanı nesne topluluklarının belirli amaçlar doğrultusunda serileştirme işlemlerine tabii tutulacağını düşünelim. Uygulama çalışma zamanı tek bir nesne bilgisi (örneğin envanterdeki kategoriler) ile çalışabileceği gibi n sayıda nesne bilgisini tutan listelere de sahip olabilir. Hatta iç içe geçen veri kümeleri de kullanılabilir. Bu tip bir nesnel oluşum aynı ortamda çalışılırken çok soru işaretine neden olmasa da, veriyi servis olarak sunduğumuzda ya da X platformunun kullanımına açtığımızda şartlara uygun bir serileştirme modeli gerekir.

Çok yaygın olarak kullanılan XML, insan (aslında programcı diyelim) gözüyle kolayca okunabilecek veri yapılarının inşa edilebilmesine olanak sağlar. Asıl ortam içerisindeki veri tipine ait canlı örnekler kolayca XML'e dönüştürülebilir ve herhangibir platform tarafından kolaylıkla değerlendirilebilir. Ancak XML veri boyutunu arttıran bir yapıya sahiptir.

Microsoft.Net platformunda geliştirme yapıyor ve hatta sadece.Net tabanlı uygulamaların haberleştiği bir dünyada yaşıyorsak ideal seçim Binary formatta serileştirme yapmak olabilir. Binary serileştrime XML'e nazaran daha az yer tutuyor olsa da az önce belirttiğimiz üzere bu model kullanıldığında platform bağımsızlık avantajı kaybedilmektedir. Diğer yandan elimizde XML'den daha az yer tutan, programcı açısından yine okunabilir nitelikte olan JSON (JavaScript Object Notation) formatı da vardır.

> Aslında veriyi basit string formatında da serileştirebiliriz. Veriye ait tüm içeriğin ardışıl olarak yazılması ile bu mümkündür. Lakin okunması çok zahmetli olacağı gibi şema yapısının (kaçıncı kolondan kaçıncıya kadar hangi alandır bilgisi gibi) da taraflara öğretilmesi gerekir. Mainframe'lerden çekilen metinsel verilerin bu tip formatlama teknikleri ile sunulduğu hizmetler halen daha vardır ancak gününümüz modern teknolojileri için XML, JSON, ProtoBuf gibi modeller ele alınmaktadır.

Bütün bunlar bir yana uzun süredir piyasada olan ve deneyimini hiçbir şekilde inkar edemeyeceğimiz Google tarafından geliştirilmiş bir serileştirme modeli daha var; Protobuf nam-ı diğer Protocol Buffers. Bu model, Google'ın kendi uygulamalarındaki sistemler arasında veri değiş tokuş işlemleri için ele aldığı bir serileştirme standardı. Var olan serileştirme tiplerine nazaran daha hızlı ve daha az yer tuttuğu ifade ediliyor. Binary formatlamayı kullanan protokol açık kaynak olarak sunulmakta. Yazının hazırlandığı tarih itibariyle C++, C#, GO, Java, Python, Javascript gibi diller tarafından da destekleniyor ([Bu adresten](https://developers.google.com/protocol-buffers/) protokol ile ilgili olarak daha detaylı bilgiye ulaşabilirsiniz)

Bu yazımızdaki amacımız ProtoBuf'ın nasıl kullanılabileceğini incelemek. Tasarlayacağımız basit bir veri yapısını, ProtoBuf standartlarında derleyecek ve GO dili ile yazılmış örnek bir kod parçasında denemeye çalışacağız. Önce sistemde gerekli hazırlıkları yapalım.

Hazırlıklar

İlk olarak sistemimize ProtoBuf derleyicisinin yüklenmesi gerekiyor. Bunun için [şu adresten](https://github.com/google/protobuf/releases/tag/v3.3.0) güncel sürümü yükleyerek ilerleyebiliriz. Ben Windows platformu için 32bitlik olan sıkıştırılmış içeriği indirdim. İçerisinde GO ile yazılıp derlenmiş protoc.exe isimli bir uygulama geldi. Bunu kullanarak proto uzantılı dosyaların derlenmesini sağlayacağız. Bu yazımızdaki örneğimizi de GO dilini kullanarak geliştireceğiz. Bize GO tarafı için işlemlerimizi kolaylaştıracak bir de paket lazım. Bu paketi github'dan aşağıdaki komut satırı ile yükleyebiliriz.

go get -u github.com/golang/protobuf/protoc-gen-go

Bu arada sistemde Go ortamının kurulu olması gerektiğini de hatırlatalım.

.proto Dosyasının Hazırlanması

Öncelikle ProtoBuf ile kullanılacak verilerin şematik olarak inşa edilmesi gerekir. Bir başka deyişle veri yapısı tasarlanmalıdır. Sonrasında tasarlanan veri yapısı bir üretim işleminden geçirilir. Üretilen sınıfa verinin GO ile ifade edilmesini sağlayacak okuma operasyonları (getter, setter metodları) ve tip tanımlamaları otomatik olarak eklenir. Üretim işlemine geçmeden önce aşağıdaki örnek kod parçasını yazarak işlemlerimize devam edelim.

```cpp
syntax="proto3";
package Southwind;

enum PlayerType{
	SEMI=0;
	PRO=1;
	MASTER=2;
}
	
message Player{
	string nickName=1;
	int32 playerId=2;
	PlayerType type=3;
	
	message Weapon{
		string name=4;
		string ability=5;
	}
	
	repeated Weapon weapons=6;
}

message Game{
	repeated Player player=1;
}
```

Proto içeriği aslında JSON formatında yazılmış ve belirli kurallara sahip olan bir yapıda tasarlanır. İlk satırda hangi ProtoBuf versiyonunun kullanılacağı ifade edilmektedir (Örneğimizde 3.0 sürümünü ele almaktayız) İçerik mutlaka bir paket adıyla tanımlanmalıdır (Southwind gibi) Veri tipleri birer message olarak ifade edilirler. Örnekte 3 mesaj ve bir enum sabiti yer almakta. Weapon, Player tipi, Player tipi de Game tipi içerisinde kullanılmaktadır. Dikkat edilmesi gereken nokta Weapon tipinin Player içerisinde dahili tip olarak tasarlanmış olmasıdır. PlayerType bir enum sabitidir (Evet yanlış görmediniz. Hani.Net servislerinde platform bağımsızlık adına kullanılmasını pek de önermediğimiz Enum sabiti kavramı burada yer almakta. İlginç değil mi? Aslında üretim işlemi sonrası o da GO'nun anlayacağı hale getirilecek)

Mesajlar içerisinde yer alan alanlara bir takım sayısal değerler atandığı görülmektedir. Bunlar serileştirme sonrası üretilecek içerikte ilgili alanların Unique olmalarını sağlayacak tanımlayıcılardır. repeated ile yazılmış ifadeler ilgili tiplerin birden fazla sayıda tekrar edileceğini belirtilir. Aynı ağaç içerisinde olmadığı sürece benzer id değerleri kullanılabilir. Örnek tasarımda string, int32 gibi veri tiplerine yer verilmiştir. Ancak daha pek çok veri tipi vardır (int64, double, float, bool vb) Detaylı bilgi için [şu adrese](https://developers.google.com/protocol-buffers/docs/proto3) bakabilirsiniz. Şimdi proto dosyasını derleyelim.

Derleme/Üretim

Yazılan Proto dosyasının kullanılması için bir üretim adımından geçirilmesi gerekmektedir. protoc aracını kullanarak yukarıda oluşturduğumuz dosyayı derleyebiliriz. Aynen aşağıdaki şekilde görüldüğü gibi.

![goproto_2.gif](images/goproto_2.gif)

Derleme sonrasında Southwind.pd.go isimli bir dosya oluşacaktır. Bu dosyanın içeriği aşağıdaki gibidir.

```cpp
// Code generated by protoc-gen-go. DO NOT EDIT.
// source: Southwind.proto

/*
Package Southwind is a generated protocol buffer package.

It is generated from these files:
	Southwind.proto

It has these top-level messages:
	Player
	Game
*/
package Southwind

import proto "github.com/golang/protobuf/proto"
import fmt "fmt"
import math "math"

// Reference imports to suppress errors if they are not otherwise used.
var _ = proto.Marshal
var _ = fmt.Errorf
var _ = math.Inf

// This is a compile-time assertion to ensure that this generated file
// is compatible with the proto package it is being compiled against.
// A compilation error at this line likely means your copy of the
// proto package needs to be updated.
const _ = proto.ProtoPackageIsVersion2 // please upgrade the proto package

type PlayerType int32

const (
	PlayerType_SEMI   PlayerType = 0
	PlayerType_PRO    PlayerType = 1
	PlayerType_MASTER PlayerType = 2
)

var PlayerType_name = map[int32]string{
	0: "SEMI",
	1: "PRO",
	2: "MASTER",
}
var PlayerType_value = map[string]int32{
	"SEMI":   0,
	"PRO":    1,
	"MASTER": 2,
}

func (x PlayerType) String() string {
	return proto.EnumName(PlayerType_name, int32(x))
}
func (PlayerType) EnumDescriptor() ([]byte, []int) { return fileDescriptor0, []int{0} }

type Player struct {
	NickName string           `protobuf:"bytes,1,opt,name=nickName" json:"nickName,omitempty"`
	PlayerId int32            `protobuf:"varint,2,opt,name=playerId" json:"playerId,omitempty"`
	Type     PlayerType       `protobuf:"varint,3,opt,name=type,enum=Southwind.PlayerType" json:"type,omitempty"`
	Weapons  []*Player_Weapon `protobuf:"bytes,6,rep,name=weapons" json:"weapons,omitempty"`
}

func (m *Player) Reset()                    { *m = Player{} }
func (m *Player) String() string            { return proto.CompactTextString(m) }
func (*Player) ProtoMessage()               {}
func (*Player) Descriptor() ([]byte, []int) { return fileDescriptor0, []int{0} }

func (m *Player) GetNickName() string {
	if m != nil {
		return m.NickName
	}
	return ""
}

func (m *Player) GetPlayerId() int32 {
	if m != nil {
		return m.PlayerId
	}
	return 0
}

func (m *Player) GetType() PlayerType {
	if m != nil {
		return m.Type
	}
	return PlayerType_SEMI
}

func (m *Player) GetWeapons() []*Player_Weapon {
	if m != nil {
		return m.Weapons
	}
	return nil
}

type Player_Weapon struct {
	Name    string `protobuf:"bytes,4,opt,name=name" json:"name,omitempty"`
	Ability string `protobuf:"bytes,5,opt,name=ability" json:"ability,omitempty"`
}

func (m *Player_Weapon) Reset()                    { *m = Player_Weapon{} }
func (m *Player_Weapon) String() string            { return proto.CompactTextString(m) }
func (*Player_Weapon) ProtoMessage()               {}
func (*Player_Weapon) Descriptor() ([]byte, []int) { return fileDescriptor0, []int{0, 0} }

func (m *Player_Weapon) GetName() string {
	if m != nil {
		return m.Name
	}
	return ""
}

func (m *Player_Weapon) GetAbility() string {
	if m != nil {
		return m.Ability
	}
	return ""
}

type Game struct {
	Player []*Player `protobuf:"bytes,1,rep,name=player" json:"player,omitempty"`
}

func (m *Game) Reset()                    { *m = Game{} }
func (m *Game) String() string            { return proto.CompactTextString(m) }
func (*Game) ProtoMessage()               {}
func (*Game) Descriptor() ([]byte, []int) { return fileDescriptor0, []int{1} }

func (m *Game) GetPlayer() []*Player {
	if m != nil {
		return m.Player
	}
	return nil
}

func init() {
	proto.RegisterType((*Player)(nil), "Southwind.Player")
	proto.RegisterType((*Player_Weapon)(nil), "Southwind.Player.Weapon")
	proto.RegisterType((*Game)(nil), "Southwind.Game")
	proto.RegisterEnum("Southwind.PlayerType", PlayerType_name, PlayerType_value)
}

func init() { proto.RegisterFile("Southwind.proto", fileDescriptor0) }

var fileDescriptor0 = []byte{
	// 246 bytes of a gzipped FileDescriptorProto
	0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xff, 0x64, 0x90, 0x41, 0x4b, 0xc3, 0x30,
	0x1c, 0xc5, 0xcd, 0x9a, 0xa5, 0xdb, 0x13, 0xb4, 0xfe, 0x41, 0x08, 0x3b, 0x95, 0x9d, 0x3a, 0x85,
	0x82, 0x15, 0xbc, 0x7b, 0x18, 0xb2, 0xc3, 0x74, 0xa4, 0x03, 0xcf, 0x9d, 0x0b, 0x58, 0x9c, 0x6d,
	0x98, 0x95, 0xd1, 0x4f, 0xea, 0xd7, 0x91, 0xfe, 0xbb, 0x75, 0x87, 0xde, 0xf2, 0x5e, 0x7e, 0xe4,
	0xfd, 0x08, 0xae, 0xd3, 0xf2, 0xb7, 0xfa, 0x3c, 0xe4, 0xc5, 0x36, 0x76, 0xfb, 0xb2, 0x2a, 0x69,
	0xdc, 0x15, 0xd3, 0x3f, 0x01, 0xb5, 0xda, 0x65, 0xb5, 0xdd, 0xd3, 0x04, 0xa3, 0x22, 0xff, 0xf8,
	0x7a, 0xcd, 0xbe, 0xad, 0x16, 0xa1, 0x88, 0xc6, 0xa6, 0xcb, 0xcd, 0x9d, 0x63, 0x6a, 0xb1, 0xd5,
	0x83, 0x50, 0x44, 0x43, 0xd3, 0x65, 0x9a, 0x41, 0x56, 0xb5, 0xb3, 0xda, 0x0b, 0x45, 0x74, 0x95,
	0xdc, 0xc6, 0xe7, 0xb5, 0xf6, 0xe1, 0x75, 0xed, 0xac, 0x61, 0x84, 0x12, 0xf8, 0x07, 0x9b, 0xb9,
	0xb2, 0xf8, 0xd1, 0x2a, 0xf4, 0xa2, 0xcb, 0x44, 0xf7, 0xe8, 0xf8, 0x9d, 0x01, 0x73, 0x02, 0x27,
	0x4f, 0x50, 0x6d, 0x45, 0x04, 0x59, 0x34, 0x72, 0x92, 0xe5, 0xf8, 0x4c, 0x1a, 0x7e, 0xb6, 0xc9,
	0x77, 0x79, 0x55, 0xeb, 0x21, 0xd7, 0xa7, 0x38, 0x7d, 0x80, 0x7c, 0x69, 0x88, 0x19, 0x54, 0xab,
	0xaa, 0x05, 0x4f, 0xde, 0xf4, 0x26, 0xcd, 0x11, 0xb8, 0xbb, 0x07, 0xce, 0xca, 0x34, 0x82, 0x4c,
	0xe7, 0xcb, 0x45, 0x70, 0x41, 0x3e, 0xbc, 0x95, 0x79, 0x0b, 0x04, 0x01, 0x6a, 0xf9, 0x9c, 0xae,
	0xe7, 0x26, 0x18, 0x6c, 0x14, 0xff, 0xe5, 0xe3, 0x7f, 0x00, 0x00, 0x00, 0xff, 0xff, 0x7c, 0x25,
	0x4e, 0xe1, 0x5e, 0x01, 0x00, 0x00,
}
```

Aslında kod okunmaya ve yorumlanmaya değer. (Bir GO paketi söz konusu ve bu tip kodları okuyup anlamaya çalışmak GO öğrenenler için önemli bir mevzu) Yazılan proto uzantılı dosyadaki tanımlamalara göre bir üretim gerçekleştirilmiştir. Yorum satırlarında dosyanın neyle üretildiği, değiştirilmemesi gerektiği, hangi mesajları sunduğu gibi bilgiler belirtilir. proto, fmt ve math gibi GO paketlerini kullanır. PlayerType ismiyle tanımlanan enum sabitinin koda bir değişmez (Constant) olarak aktarıldığında dikkat edelim. Aslında GO için bu enum sabiti int32 tipinden bir değişkendir. Bununla birlikte enum sabitinin içeriğine isimle (name) veya değerle (value) ulaşabilmek için bir map değişkeni (PlayerTypename,PlayerTypevalue) tanımlandığı görülür. Kodda enum sabiti olmak üzere diğer mesajlar için metodlar yazıldığına da dikkat edelim. Player ve Game birer yapı (struct) olarak oluşturulmuşlardır. Alanların değerlerini okumak için Get kelimesi ile başlayan metodlar vardır. Oyuncunun sahip olabileceği silahlar için Player tipinden bir slice değişkeni yer almaktadır. Oyun sahasındaki oyuncular için de benzer şekilde Game yapısı içinde Player tipinden bir slice değişkenini işaret eden pointer'a yer verilmiştir. Bu bir GO uygulamasına ilave edilerek kullanılacak bir paket olduğundan pek tabii main fonksiyonu içermez. Ancak başlangıçta ilgili mesaj tiplerinin proto katmanına enjekte edilmesi için bir takım fonksiyon çağrıları gerçekleşir.

Ana Uygulama

Gelelim bu içeriği uygulamada nasıl kullanabileceğimize. Öncelikle oluşan Southwind.pb.go dosyasını paket olarak konuşlandırmamız lazım. Sistemdeki GOPATH bildirimine göre uygun bir klasör içerisine atabiliriz. Ben GOPATH'in işaret ettiği src klasöründe aşağıdaki gibi bir yapılandırma oluşturdum. Southwind.pd.go dosyasının Southwind isimli bir klasörde olması mühim. Aksi halde kod ilgili paketin yerini bulamayacaktır.

C->Go Works
----Samples
--------------Src
------------------Message
----------------------------Southwind
----------------------------------------Southwind.pb.go

Şimdi istediğimi lokasyondan bu paketi kullanabiliriz. İşte örnek bir kod parçası.

```cpp
package main

import (
	"fmt"

	data "message/Southwind"

	"github.com/golang/protobuf/proto"
)

func main() {
	taverna := data.Game{Player: []*data.Player{
		{
			NickName: "Leksar",
			PlayerId: 10,
			Type:     data.PlayerType_SEMI,
			Weapons: []*data.Player_Weapon{
				{Name: "Sword", Ability: "High level sword"},
				{Name: "Machine Gun", Ability: "7.65mm"},
			},
		},
		{
			NickName: "Valira",
			PlayerId: 12,
			Type:     data.PlayerType_SEMI,
			Weapons: []*data.Player_Weapon{
				{Name: "Poison Bottle", Ability: "Dangeres green"},
			},
		},
	},
	}
	sData, err := proto.Marshal(&taverna)
	if err != nil {
		fmt.Println(err.Error())
	} else {
		fmt.Println(sData)
		fmt.Println(string(sData))
	}

	dsData := &data.Game{}
	err = proto.Unmarshal(sData, dsData)
	if err != nil {
		fmt.Println(err.Error())
	} else {
		for _, p := range dsData.Player {
			fmt.Println(p.NickName)
			for _, w := range p.Weapons {
				fmt.Printf("\t%s\t%s\n", w.Name, w.Ability)
			}
		}
	}
}
```

Öncelikle kodda neler yaptığımız bir bakalım. ProtoBuf formatında serileştirme ve ters serileştirme işlemleri için github.com/golang/protobuf/proto paketinde yer alan proto tipinin Marshal ve Unmarshal metodlarından yararlanılıyor. taverna isimli değişken içerisinde iki oyuncu ve bu oyuncuların silahlarına ait test verileri yer almakta. Marshal metodu ile serileştirilen içeriği hem byte array hem de string tipine dönüştürülmüş olarak ekrana bastırmaktayız. Sonrasında da serileştirilmiş bu veri içeriğinden yeni bir Game örneğine ters serileştirme yaparak elde edilen verileri yazdırmaktayız. Çalışma zamanı sonuçları aşağıdaki ekran görüntüsündeki gibidir.

![goproto_4.gif](images/goproto_4.gif)

Tabii ki serileştirilen içerik fiziki bir dosyaya çıkılabilir veya ağ üzerinden bir kanala yazdırılabilir. Daha az yer kapladığı ortada. Yine de gerçek benchmark testleri ve farklı serileştirme formatları ile karşılaştırılması için interneti dolaşmakta yarar var. Üretilen serileştirilmiş içeriğe bakıldığında ise sadece verinin tutulduğu görülmektedir. Tahmin edeceğiniz üzere verinin şeması paket olarak eklediğimiz Southwind.pd.go içerisinde yer alıyor. Dolayısıyla Marshal ve Unmarsal işlemlerinde bu paketten yararlanılmakta.

Böylece geldik bir GoLang maceramızın daha sonuna. Bu yazımızda Google'ın geliştirdiği serileştirme protokolü Proto Buffer'ın bir Go uygulamasında nasıl kullanılabileceğini incelemeye çalıştık. Size tavsiyem diğer dil paketlerini de işin içine katarak geliştirme yapmaya çalışmanız olacaktır. Bir başka yazımızda görüşünceye dek hepinize mutlu günler dilerim.
