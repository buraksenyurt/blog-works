---
title: "Tek Fotoluk İpucu 79– svcutil ile Contract-First Development"
pubDate: 2013-02-11 02:56:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - WCF
---

# Tek Fotoluk İpucu 79– svcutil ile Contract-First Development
Merhaba Arkadaşlar,

WCF 4.5 tarafında gelen yeniliklerden birisi de svcutil komut satırına eklenen servicecontract (ya da kısa haliyle sc) parametresidir. Bu parametre sayesinde bir WSDL dokümanından (ve beraberinde kullandığı XSD’ ler var ise onlardan) servis sözleşmesinin (Service Contract) elde edilebilmesi mümkündür. Tek yapmanız gereken aşağıdakine benzer şekilde sc parametresini kullanmanız olacaktır.

[![tfi79_1](images/tfi79_1_thumb.png)](images/tfi79_1.png)

Bu örnekte WSDL dökümanı XSD’ leri de bünyesinde barındırmaktadır. Eğer XSD’ ler harici dosyalarda tutulmaktaysalar onları da komut satırında belirtmeniz gerekecektir. Aşağıdaki fotoğrafta görüldüğü gibi

![Winking smile](images/wlEmoticon-winkingsmile_140.png)

[![tfi79_2](images/tfi79_2_thumb.png)](images/tfi79_2.png)

Başka bir ipucunda görüşmek dileğiyle

![Winking smile](images/wlEmoticon-winkingsmile_140.png)
