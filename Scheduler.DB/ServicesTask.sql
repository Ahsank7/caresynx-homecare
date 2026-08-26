INSERT INTO [dbo].[tblServicesTask] ([ScheduleId],[EndTime],[Date],[StartTime],[ClientId],[ServiceProviderId],[UpdatedBy],[CreatedDate],[CreatedBy],[UpdatedDate] ,[Status],[Notes],[CheckIn],[CheckOut])
     select 1,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 2,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 3,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 4,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 5,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 6,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 7,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 8,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 9,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 10,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 11,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
           
     union
	 select 1,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 2,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 3,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 4,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 5,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 6,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 7,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 8,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 9,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 10,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 11,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 1,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),1,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 2,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 3,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 4,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 5,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 6,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 7,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 8,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 9,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 10,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 11,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),2,'Notes',GETUTCDATE(),GETUTCDATE()
          union
	 select 1,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 2,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 3,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 4,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 5,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 6,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 7,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 8,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union																																																		 
	 select 9,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 10,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
     union
	 select 11,GETUTCDATE(),GETDATE(),GETUTCDATE(),(select top 1 Id from tblClient),(select top 1 Id from tblServiceProvider),(select top 1 Id from tblStaff),GETDATE(),(select top 1 Id from tblStaff),GETDATE(),3,'Notes',GETUTCDATE(),GETUTCDATE()
      

  

 




--insert into [dbo].[tblTaskStatus]
--values(1,'Pending')
--values(2,'Delayed')
--values(3,'Completed')