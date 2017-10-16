	trigger Update_AccountProduct on Opportunity (after insert, after update)
	{   
		
		for(Opportunity opp : trigger.new)
		{		
			if (opp.StageName != 'Closed Won Approved' &&  opp.StageName != 'Closed Won')
			{
				return;
			}		
			
			string account_name = [select name from account where id =: opp.AccountId][0].name;		
			system.debug('>>Account id: ' + opp.AccountId);
			system.debug('>>Account name: ' + account_name);
			
			
			//Delete exsiting old Products monthly revenue recognition
			list<AccountProducts__c	> pmrr_ls = new list<AccountProducts__c	>([select id,name from AccountProducts__c where Account__c =: opp.AccountId AND opportunity__c =: trigger.new[0].id]);
				
		
			//retrive the Oli's
			list<opportunityLineItem> opli = new list<opportunityLineItem>([select Product_line_item_id__c ,product2.name, opportunityId, New_Renewal__c, ProductCode, Line_Item_Price__c, Line_Item_Type__c,Line_Item_Start_Date__c, Support_Subscription_calc_end_date__c ,product2.Code__c, Support_Subscription_end_date__c, ListPrice, Annual_Support__c, Subscription_Period_Months__c, UnitPrice, Quantity, Discount,MRR_Period__c, Subscription_Support_Months__c FROM opportunitylineitem where  opportunityId =: trigger.new[0].id]);
			
			
			list<AccountProducts__c	> account_products = new list<AccountProducts__c>();			
			
			for(opportunityLineItem oli : opli)
			{
				
				system.debug('>>>>Add Oli as Account Product for: ' + oli.product2.name);
				system.debug('>>>oli.UnitPrice:' + oli.UnitPrice);                  
				system.debug('>>>oli.Line_Item_Price__c :' + oli.Line_Item_Price__c );			
				system.debug('>>>>item Subscription_Period_Months__c: '+ oli.Subscription_Period_Months__c);
												
				//to_unite = false;
				
				if(oli.Line_Item_Type__c == 'Monthly Subscription' || oli.Line_Item_Type__c == 'Perpetual' || oli.Line_Item_Type__c == 'Annual Support')
				{
					system.debug('>>>Start create new Account Prosuce object');
								
					AccountProducts__c account_product = new AccountProducts__c(
					Name = oli.product2.name,
					Account__c = opp.AccountId,
					Annual_Support__c = oli.Annual_Support__c,								
					Line_item_Price__c = oli.Line_item_Price__c,
					Line_Item_Start_Date__c = oli.Line_Item_Start_Date__c,
					Line_Item_Type__c = oli.Line_Item_Type__c,
					MRR_Period__c = oli.MRR_Period__c,
					New_Renewal__c = oli.New_Renewal__c,
					opportunity__c = oli.opportunityId,
					ProductCode__c = oli.product2.Code__c,
					Product_line_item_id__c = oli.Product_line_item_id__c,
					Quantity__c = oli.quantity,
					sales_price__c = oli.UnitPrice,
					Support_Subscription_end_date__c = oli.Support_Subscription_calc_end_date__c
					);
			
					system.debug('>>> created AC - '+ account_product);
					account_products.add(account_product);		
				}
				
			}
			
			delete pmrr_ls;
			insert account_products;
			system.debug('>>>>Update_AccountProduct trigger update db'); 
		}
		
			
		  
	}