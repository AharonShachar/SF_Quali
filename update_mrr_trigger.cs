trigger update_mrr on Opportunity (after insert, after update)
{   
    
    for(Opportunity opp : trigger.new)
	{		
		if (opp.StageName != 'Closed Won Approved')
		{
			return;
		}
		
		//getUserInfoResult result = connection.getUserInfo();
		system.debug('>>>>getDefaultCurrency: '+ 	UserInfo.getDefaultCurrency());
		system.debug('>>>>isMultiCurrencyOrganization: '+ 	UserInfo.isMultiCurrencyOrganization());
		//system.debug('>>>>orgDefaultCurrencyIsoCode: '+ 	result.orgDefaultCurrencyIsoCode);

		 
		user ariel = new list<user>([select CurrencyIsoCode,name, DefaultCurrencyIsoCode from user where alias =: 'kuper'])[0];
		system.debug('>>>>Ariel: '+ 	ariel.name);
		//ariel.CurrencyIsoCode = 'ILS';
		ariel.DefaultCurrencyIsoCode = 'USD';		
		update ariel;
		ariel = new list<user>([select CurrencyIsoCode,name, DefaultCurrencyIsoCode from user where Alias =: 'kuper'])[0];
		system.debug('>>>>DefaultCurrencyIsoCode: '+ 	ariel.DefaultCurrencyIsoCode);
		
		MRR_Manager mrr_manager = new MRR_Manager(opp);        
    
		//Delete exsiting old Products monthly revenue recognition
		list<Products_monthly_revenue_recognition__c> pmrr_ls = new list<Products_monthly_revenue_recognition__c>([select id, Name, Opportunity__c from Products_monthly_revenue_recognition__c where Opportunity__c =: trigger.new[0].id AND 
		(Line_Item_Type__c='Monthly Subscription' OR Line_Item_Type__c='Perpetual' OR Line_Item_Type__c='Annual Support')]);
		
		delete pmrr_ls;    
    
		//retrive the Oli's
		list<opportunityLineItem> opli = new list<opportunityLineItem>([select Product_line_item_id__c ,product2.name, opportunityId, New_Renewal__c, ProductCode, Line_Item_Price__c, Line_Item_Type__c,Line_Item_Start_Date__c, Support_Subscription_calc_end_date__c , Support_Subscription_end_date__c, ListPrice, Subscription_Period_Months__c, UnitPrice, Quantity, Discount, Subscription_Support_Months__c FROM opportunitylineitem where opportunityId =: trigger.new[0].id]);

		//system.debug('>>>>op type: ' 	+ [select Type.Id from Opportunity  WHERE Id =: trigger.new[0].id]);
		
		
		for(opportunityLineItem oli : opli)
		{	
			system.debug('>>>>Calling to ConvertLIneItemToMRR for line: ' + oli.product2.name);
			mrr_manager.ConvertLIneItemToMRR(oli,opp);
		}
	}
    
        
      
}