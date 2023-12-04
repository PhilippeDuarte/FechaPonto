<head></head>
<body>
    <h2>Sistema FechaPonto</h2>
    <h3>Sobre FechaPonto</h3>
    <p>Este sistema faz parte do teste de recrutamento proposto pela empresa Auvo.</p>
    <p>O objetivo do teste é criar um sistema para ler arquivos de ponto em formato .csv em um diretório definido pelo usuário.</p>
    <h3>Recursos usados no desenvolvimento</h3>
    <p>Este sistema foi desenvolvido usando o <b>Microsoft Visual Studio Community 2022 V17.4.4.</b>, usando a estrutura <b>ASP. Net Core 6.0</b>.</p>
    <h3>Considerações</h3>
    <p>Durante o desenvolvimento deste sistema, foi considerado que o arquivo a ser lido considera um dia de falta com valores de entrada e saída iguais a 00:00:00. Essa decisão foi tomada para considerar o ValorPago a ser debitado por hora de ausência. </p>
    <h3>Como usar</h3>
    <ul>
        <li>Ao carregar o site, será exibido um formulário para inserir o caminho para o diretório onde se encontram os arquivos de ponto;</li>
        <li>Digitar o caminho do diretório onde se encontra o arquivo. Exemplo: C:\Ponto;</li>
        <li>Clicar em Buscar.</li>
    </ul>
    <h3>Precauções</h3>
    <ul>
        <li>Certificar-se que o diretório existe;</li>
        <li>Certificar-se que os arquivos possuem nome válido. Exemplo: <b>Financeiro-Agosto-2023.csv</b>;</li>
        <li>Certificar-se que os valores nos arquivos estão no formato esperado pelo sistema. Exemplo: <b>3;Philippe Duarte;R$ 32,45;01/12/2023;07:00;18:59;12:00 - 13:00</b>;</li>
        <li>O Relatório ou a mensagem de erro serão gerados em uma nova aba do navegador. </li>
    </ul>
</body>