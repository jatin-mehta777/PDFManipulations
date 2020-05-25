CREATE TABLE [dbo].[FileDetails](  
    [Id] [int] IDENTITY(1,1) NOT NULL,  
    [FileName] [varchar](60) NULL,  
    [FileContent] [varbinary](max) NULL  
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] 

Create Procedure [dbo].[AddFileDetails]  
(  
@FileName varchar(60),  
@FileContent varBinary(Max)  
)  
as  
begin  
Set NoCount on  
Insert into FileDetails values(@FileName,@FileContent)  
  
End		

CREATE Procedure [dbo].[GetFileDetails]  
(  
@Id int=null  
  
  
)  
as  
begin  
  
select Id,FileName,FileContent from FileDetails  
where Id=isnull(@Id,Id)  
End 