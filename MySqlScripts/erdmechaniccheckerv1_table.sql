CREATE TABLE Seller (
  SellerId          int(10) NOT NULL AUTO_INCREMENT, 
  Username          varchar(255) NOT NULL UNIQUE, 
  FirstName         varchar(255) NOT NULL, 
  LastName          varchar(255) NOT NULL, 
  Email             varchar(255) NOT NULL UNIQUE, 
  PasswordHash      varchar(255), 
  AccountType       varchar(255) NOT NULL, 
  IsApproved        tinyint(1) DEFAULT 0 NOT NULL, 
  CompanyName       varchar(255) NOT NULL UNIQUE, 
  BusinessPhone     char(10) NOT NULL UNIQUE, 
  CompanyLogoUrl    varchar(2000) NOT NULL, 
  WebsiteUrl        varchar(2000), 
  Application       text, 
  ApplicationDate   datetime DEFAULT CURRENT_TIMESTAMP NOT NULL, 
  ApprovalDate      datetime NULL, 
  ActivationCode    varchar(255), 
  ResetPasswordCode varchar(255), 
  PRIMARY KEY (SellerId), 
  CONSTRAINT seller_email_ck 
    CHECK (Email REGEXP '^([a-zA-Z0-9]+(?:[.-]?[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:[.-]?[a-zA-Z0-9]+)*\.[a-zA-Z]{2,7})$'), 
  CONSTRAINT seller_accounttype_ck 
    CHECK (AccountType IN ('Store', 'Individual')), 
  CONSTRAINT seller_approvaldate_ck 
    CHECK (ApprovalDate >= ApplicationDate), 
  CONSTRAINT seller_businessphone_ck 
    CHECK (BusinessPhone REGEXP '^[0-9]{10}$'), 
  CONSTRAINT seller_companylogourl_ck 
    CHECK (CompanyLogoUrl REGEXP '^https?://'), 
  CONSTRAINT seller_websiteurl_ck 
    CHECK (WebsiteUrl REGEXP '^https?://'));
CREATE TABLE Product (
  ProductId   int(10) NOT NULL AUTO_INCREMENT, 
  SellerId    int(10) NOT NULL, 
  Category    varchar(255) NOT NULL, 
  Title       varchar(255) NOT NULL, 
  Price       decimal(13, 2), 
  Description text NOT NULL, 
  ImageUrl    varchar(2000) NOT NULL, 
  ProductUrl  varchar(2000), 
  IsQuote     tinyint(1) DEFAULT 0 NOT NULL, 
  IsVisible   tinyint(1) DEFAULT 1 NOT NULL, 
  PRIMARY KEY (ProductId), 
  INDEX (SellerId), 
  CONSTRAINT product_category_ck 
    CHECK (Category IN ('Item', 'Service')), 
  CONSTRAINT product_producturl_ck 
    CHECK (ProductUrl REGEXP '^https?://'), 
  CONSTRAINT product_imageurl_ck 
    CHECK (ImageUrl REGEXP '^https?://'), 
  CONSTRAINT product_price_ck 
    CHECK (Price >= 0.01));
CREATE TABLE Address (
  AddressId  int(10) NOT NULL AUTO_INCREMENT, 
  SellerId   int(10) NOT NULL, 
  Address    varchar(255) NOT NULL, 
  City       varchar(255) NOT NULL, 
  Province   char(2) NOT NULL, 
  PostalCode char(7) NOT NULL, 
  PRIMARY KEY (AddressId), 
  INDEX (SellerId), 
  CONSTRAINT address_province_ck 
    CHECK (Province IN ('AB', 'BC', 'MB', 'NB', 'NL', 'NT', 'NS', 'NU', 'ON', 'PE', 'QC', 'SK', 'YT')), 
  CONSTRAINT address_postalcode_ck 
    CHECK (PostalCode REGEXP '^([ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ])\ ?([0-9][ABCEGHJKLMNPRSTVWXYZ][0-9])$'));
CREATE TABLE APIKey (
  APIKeyId   int(10) NOT NULL AUTO_INCREMENT, 
  Service    varchar(255) NOT NULL, 
  APIKey     varchar(255) NOT NULL, 
  KeyOwner   varchar(255) NOT NULL, 
  IsEnabled  tinyint(1) DEFAULT 0 NOT NULL, 
  Quota      int(10) NOT NULL, 
  ActiveDate datetime DEFAULT CURRENT_TIMESTAMP NOT NULL, 
  ExpireDate datetime NULL, 
  APIHost    varchar(255), 
  PRIMARY KEY (APIKeyId), 
  CONSTRAINT apikey_service_apikey_uq 
    UNIQUE (Service, APIKey), 
  CONSTRAINT apikey_service_keyowner_uq 
    UNIQUE (Service, KeyOwner), 
  CONSTRAINT apikey_service_ck 
    CHECK (Service IN ('DeveloperAPI GoogleMaps', 'RapidAPI Amazon', 'RapidAPI Alibaba', 'DeveloperAPI Ebay', 'DeveloperAPI SendGrid')), 
  CONSTRAINT apikey_quota_ck 
    CHECK (Quota >= 0), 
  CONSTRAINT apikey_keyowner_ck 
    CHECK (KeyOwner IN ('Michael', 'Emmanuel', 'Ibrahim', 'Nusrat', 'Sanjib', 'Shaminda', 'Shaniquo', 'MechanicChecker')), 
  CONSTRAINT apikey_expiredate_ck 
    CHECK (ExpireDate > ActiveDate));
