create schema Ticketing;

create table Ticketing.Region (
	Id            int             identity (1, 1) not null,
    Name          varchar (200)   not null,
    constraint PK_Region primary key clustered (Id asc)
);

create table Ticketing.Company (
	Id            int             identity (1, 1) not null,
    [Name]        varchar (200)   not null,
	Email         varchar (200)   not null,
	EmailAddress  varchar (200)   not null,
    EmailPassword varchar (200)   not null,
	Logo		  varbinary(max)  null,
	SmtpPort      smallint        not null,
    SmtpAddress   varchar (200)   not null,
    EnableSsl     bit             not null,
    Pop3Port      smallint        not null,
    Pop3Address   varchar (200)   not null,
	RegionId      int             not null,
    constraint PK_Company primary key clustered (Id asc),
	constraint FK_Company_Region foreign key (RegionId) references Ticketing.Region (Id)
);

create table Ticketing.[Level] (
	Id                 int             identity (1, 1) not null,
    FirstName          nvarchar (200)  not null,
	FirstHours         int             not null,
	SecondName         nvarchar (200)  not null,
	SecondHours        int             not null,
	ThirdName          nvarchar (200)  not null,
	CompanyId		   int		       not null,
	constraint PK_Level primary key clustered (Id asc),
	constraint FK_Level_Company foreign key (CompanyId) references Ticketing.Company (Id)
);

create table Ticketing.[User] (
    Id             	  int             identity (1, 1) not null,
    [Name]            varchar (100)   not null,
    Email          	  varchar (50)    not null,
    Mobile         	  varchar (50)    null,
    UserName       	  varchar (50)    not null,
    [Role]         	  tinyint         not null,
	[Level]           tinyint         null,
    [Password]     	  varbinary (500) not null,
    ProfilePhoto   	  varbinary (max) null,
	CompanyId		  int		      null,
    constraint PK_User primary key clustered (Id asc),
	constraint FK_User_Company foreign key (CompanyId) references Ticketing.Company (Id),
);

create table Ticketing.Ticket (
    Id                 int             identity (1, 1) not null,
    [Name]             nvarchar (200)  not null,
    Email              varchar (200)   not null,
    Mobile             varchar (20)    null,
    [Subject]          nvarchar (500)  not null,
    [Message]          nvarchar (4000) null,
    [Status]           tinyint         not null,
    AssignedUserId     int             null,
	UpdateUserId       int             null,
    CreateDate         datetime        constraint [DF_Ticket_CreateDate] default (getdate()) null,
    UpdateDate         datetime        null,
    CompanyId          int             null,
	[Level]            tinyint         not null,
    Attachment         varbinary (max) null,
    AttachmentFileName nvarchar (200)  null,
    [Key]              varchar (10)    null,
    constraint PK_Ticket primary key clustered (Id asc),
    constraint FK_Tikcet_AssignedUser foreign key (AssignedUserId) references Ticketing.[User] (Id),
    constraint FK_Tikcet_CreateByUser foreign key (UpdateUserId) references Ticketing.[User] (Id),
    constraint FK_Tikcet_Company foreign key (CompanyId) references Ticketing.Company (Id)
);

create table Ticketing.TicketReply (
    Id                 int             identity (1, 1) not null,
    TicketId           int             not null,
    [Message]          nvarchar (4000) null,
    [Status]           tinyint         not null,
    Attachment         varbinary (max) null,
    AttachmentFileName nvarchar (200)  null,
    IsInternal         bit             not null,
    CreateByUserId     int             null,
    CreateDate         datetime        constraint [DF_TicketReply_CreateDate] default (getdate()) null,
    constraint PK_TicketReply primary key clustered (Id asc),
    constraint FK_TicketReply_TicketId foreign key (TicketId) references Ticketing.Ticket (Id),
    constraint FK_TicketReply_CreateByUser foreign key (CreateByUserId) references Ticketing.[User] (Id)
);

insert into Ticketing.Region ([Name]) values ('Asia');
insert into Ticketing.Region ([Name]) values ('Europe');
insert into Ticketing.Region ([Name]) values ('Middle East');
insert into Ticketing.Region ([Name]) values ('North America');
insert into Ticketing.Region ([Name]) values ('South America');
insert into Ticketing.Region ([Name]) values ('Australia');