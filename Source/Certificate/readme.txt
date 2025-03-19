How-to generate self-signed certificate:
1. openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365
2. openssl pkcs12 -inkey key.pem -in cert.pem -export -out cert.pfx

Import the certificate (pfx) into the Windows Certificate Store on the target machine:
1. Open Certificate Manager for the Local Machine (run certlm.msc)
2. Open Certificates (Local Computer) -> Trusted Root Certification Authorities
3. Right-click on Trusted Root Certification Authorities, then click on All Tasks -> Import...
4. Find and import your certificate
5. Do the same steps 2-4 for Certificates (Local Computer) -> Personal
6. Run the following command in cmd.exe:
    netsh http add sslcert ipport=0.0.0.0:{SERVICE_HTTPS_PORT} certhash=110000000000003ed9cd0c315bbb6dc1c08da5e6 appid={00112233-4455-6677-8899-AABBCCDDEEFF}

{SERVICE_HTTPS_PORT} - specify AccessControlAdapter service https port
certhash - the certificate thumbprint (get it by double click on the certificate in certlm.msc and then go to the Details tab)
appid - random unique GUID for the netsh rule identification