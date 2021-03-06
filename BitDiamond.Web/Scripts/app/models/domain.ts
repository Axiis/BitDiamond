//     This code was generated by a Reinforced.Typings tool. 
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.


module Pollux.Models {
	export interface IPolluxEntity<Key>
	{
		EntityId: Key;
		CreatedOn: Apollo.Models.JsonDateTime;
		ModifiedOn?: Apollo.Models.JsonDateTime;
	}
	export interface ICredentialMetadata
	{
		Name: string;
		Access: Pollux.Models.Access;
	}
	export interface IAddressData extends IPolluxEntity<number>
	{
		Street: string;
		City: string;
		StateProvince: string;
		Country: string;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
	}
	export interface IBioData extends IPolluxEntity<number>
	{
		FirstName: string;
		MiddleName: string;
		LastName: string;
		Dob?: Apollo.Models.JsonDateTime;
		Gender: Pollux.Models.Gender;
		Nationality: string;
		StateOfOrigin: string;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
	}
	export interface IContactData extends IPolluxEntity<number>
	{
		Phone: string;
		AlternatePhone: string;
		PhoneConfirmed: boolean;
		Email: string;
		AlternateEmail: string;
		EmailConfirmed: boolean;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
	}
	export interface ICorporateData extends IPolluxEntity<number>
	{
		CorporateName: string;
		CorporateId: string;
		Description: string;
		IncorporationDate?: Apollo.Models.JsonDateTime;
		Status: number;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
	}
	export interface IUser extends IPolluxEntity<string>
	{
		UserId: string;
		Status: number;
		UId: any;
	}
	export interface IUserData extends IPolluxEntity<number>
	{
		Data: string;
		Name: string;
		Type: any;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
	}
	export interface ICredential extends IPolluxEntity<number>
	{
		Metadata: Pollux.Models.ICredentialMetadata;
		Value: string;
		SecuredHash: string;
		ExpiresIn?: number;
		Expires?: Apollo.Models.JsonDateTime;
		Status: Pollux.Models.CredentialStatus;
		Tags: string;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
	}
	export interface IPermission extends IPolluxEntity<number>
	{
		ResourceSelector: string;
		Role: Pollux.Models.IRole;
		RoleId: string;
		Effect: Pollux.Models.Effect;
	}
	export interface IResource extends IPolluxEntity<number>
	{
		Path: string;
		Description: string;
	}
	export interface IRole extends IPolluxEntity<string>
	{
		RoleName: string;
		Name: string;
		Value: any;
	}
	export interface IUserRole extends IPolluxEntity<number>
	{
		UserId: string;
		RoleName: string;
	}
	export enum Gender { 
		Female = 0, 
		Male = 1, 
		Other = 2, 
	}
	export enum Access { 
		Public = 0, 
		Secret = 1, 
	}
	export enum CredentialStatus { 
		Active = 0, 
		Inactive = 1, 
	}
	export enum Effect { 
		Deny = 0, 
		Grant = 1, 
	}
	export enum CombinationMethod { 
		All = 0, 
		Any = 1, 
	}
}
module BitDiamond.Models {
	export interface IBaseModel<IdType>
	{
		Id: IdType;
		CreatedOn: Apollo.Models.JsonDateTime;
		ModifiedOn?: Apollo.Models.JsonDateTime;
	}
	export interface IBitcoinAddress extends IBaseModel<number>
	{
		BlockChainAddress: string;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
		OwnerRef: BitDiamond.Models.IReferralNode;
		IsVerified: boolean;
		IsActive: boolean;
	}
	export interface IBlockChainTransaction extends IBaseModel<number>
	{
		Sender: BitDiamond.Models.IBitcoinAddress;
		SenderId: number;
		Receiver: BitDiamond.Models.IBitcoinAddress;
		ReceiverId: number;
		TransactionHash: string;
		LedgerCount: number;
		Amount: number;
		Status: BitDiamond.Models.BlockChainTransactionStatus;
		ContextId: string;
		ContextType: string;
	}
	export interface IContextVerification extends IBaseModel<number>
	{
		Target: Pollux.Models.IUser;
		TargetId: string;
		VerificationToken: string;
		Verified: boolean;
		Context: string;
		ExpiresOn: Apollo.Models.JsonDateTime;
	}
	export interface IBitLevel extends IBaseModel<number>
	{
		Donation: BitDiamond.Models.IBlockChainTransaction;
		DonationId?: number;
		Level: number;
		SkipCount: number;
		DonationCount: number;
		Cycle: number;
		User: Pollux.Models.IUser;
		UserId: string;
	}
	export interface INotification extends IBaseModel<number>
	{
		Title: string;
		Message: string;
		Type: BitDiamond.Models.NotificationType;
		Seen: boolean;
		Context: string;
		ContextId: string;
		Target: Pollux.Models.IUser;
		TargetId: string;
	}
	export interface IPost extends IBaseModel<number>
	{
		Title: string;
		Message: string;
		Owner: Pollux.Models.IUser;
		OwnerId: string;
		Context: string;
		ContextId: string;
		Status: BitDiamond.Models.PostStatus;
	}
	export interface IReferralNode extends IBaseModel<number>
	{
		User: Pollux.Models.IUser;
		UserId: string;
		ReferenceCode: string;
		ReferrerCode: string;
		Referrer: BitDiamond.Models.IReferralNode;
		Referrals: BitDiamond.Models.IReferralNode[];
		UplineCode: string;
		Upline: BitDiamond.Models.IReferralNode;
		UserBio: Pollux.Models.IBioData;
		UserContact: Pollux.Models.IContactData;
		ProfileImageUrl: string;
		DirectDownlines: BitDiamond.Models.IReferralNode[];
	}
	export interface ISystemSetting extends IBaseModel<number>
	{
		Data: string;
		Name: string;
		Type: any;
	}
	export interface IUserLogon extends IBaseModel<number>
	{
		Client: any;
		Location: string;
		OwinToken: string;
		Invalidated: boolean;
		Locale: string;
		TimeZone: string;
		User: Pollux.Models.IUser;
		UserId: string;
	}
	export enum ScheduleInterval { 
		Minutely = 0, 
		Hourly = 1, 
		Daily = 2, 
		Weekly = 3, 
		Monthly = 4, 
		Yearly = 5, 
	}
	export enum AccountStatus { 
		Active = 0, 
		InActive = 1, 
		Blocked = 2, 
	}
	export enum BlockChainTransactionStatus { 
		Unverified = 0, 
		Verified = 1, 
	}
	export enum NotificationType { 
		Info = 0, 
		Error = 1, 
		Warning = 2, 
		Success = 3, 
	}
	export enum PostStatus { 
		Draft = 0, 
		Published = 1, 
		Archived = 2, 
	}
}
