// Enums - numeric values matching C# backend
export enum UserRole {
  Parent = 0,
  Child = 1,
  Relative = 2,
}

export enum TransactionType {
  Deposit = 0,
  Withdrawal = 1,
  Allowance = 2,
  Gift = 3,
  Interest = 4,
  Correction = 5,
}

export enum PaymentInterval {
  Weekly = 1,
  Biweekly = 2,
  Monthly = 3,
}

export enum MoneyRequestStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Withdrawn = 3,
}

// Auth DTOs
export interface UserDto {
  id: string;
  email: string | null;
  nickname: string;
  role: UserRole;
  mfaEnabled: boolean;
  familyId: string | null;
  securityTutorialCompleted: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ChildLoginRequest {
  familyCode: string;
  nickname: string;
  pin: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface MfaRequiredResponse {
  mfaRequired: boolean;
  mfaToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  nickname: string;
}

export interface RegisterResponse {
  userId: string;
  mfaSetupRequired: boolean;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface VerifyTotpRequest {
  mfaToken: string;
  code: string;
}

export interface SetupTotpResponse {
  setupToken: string;
  secretKey: string;
  qrCodeUri: string;
}

export interface BackupCodesResponse {
  codes: string[];
}

// Account DTOs
export interface AccountDto {
  id: string;
  userId: string;
  ownerName: string;
  balance: number;
  interestEnabled: boolean;
  interestRate: number | null;
  createdAt: string;
}

export interface TransactionDto {
  id: string;
  accountId: string;
  amount: number;
  type: TransactionType;
  description: string | null;
  category: string | null;
  createdByUserId: string | null;
  createdByUserName: string | null;
  balanceAfter: number;
  createdAt: string;
}

export interface DepositRequest {
  amount: number;
  description?: string;
}

export interface WithdrawRequest {
  amount: number;
  description?: string;
  category?: string;
}

// Predefined expense categories for children
export const ExpenseCategories = [
  'Süßigkeiten',
  'Spielzeug',
  'Kino',
  'Bücher',
  'Kleidung',
  'Spiele',
  'Essen & Trinken',
  'Geschenke',
  'Sonstiges',
] as const;

export interface GiftRequest {
  amount: number;
  message?: string;
}

export interface SetInterestRequest {
  interestRate: number;
  interestEnabled: boolean;
}

// Family DTOs
export interface FamilyDto {
  id: string;
  name: string;
  familyCode: string;
  createdAt: string;
}

export interface FamilyMemberDto {
  id: string;
  nickname: string;
  role: UserRole;
  email: string | null;
}

export interface ChildDto {
  id: string;
  nickname: string;
  accountId: string | null;
  balance: number;
}

export interface CreateFamilyRequest {
  name: string;
}

export interface AddChildRequest {
  nickname: string;
  pin: string;
}

export interface AddChildResponse {
  userId: string;
  accountId: string;
}

export interface InviteMemberRequest {
  email: string;
  relationshipDescription?: string;
}

export interface InvitationDto {
  id: string;
  email: string;
  familyName: string;
  invitedByName: string;
  relationshipDescription: string | null;
  createdAt: string;
  expiresAt: string;
}

// RecurringPayment DTOs
export interface RecurringPaymentDto {
  id: string;
  accountId: string;
  childName: string;
  amount: number;
  description: string | null;
  interval: PaymentInterval;
  dayOfWeek: number | null;
  dayOfMonth: number | null;
  isActive: boolean;
  nextExecutionDate: string;
  lastExecutionDate: string | null;
  createdAt: string;
}

export interface CreateRecurringPaymentRequest {
  accountId: string;
  amount: number;
  description?: string;
  interval: PaymentInterval;
  dayOfWeek?: number;
  dayOfMonth?: number;
}

export interface UpdateRecurringPaymentRequest {
  amount?: number;
  description?: string;
  interval?: PaymentInterval;
  dayOfWeek?: number;
  dayOfMonth?: number;
  isActive?: boolean;
}

// MoneyRequest DTOs
export interface MoneyRequestDto {
  id: string;
  childUserId: string;
  childName: string;
  amount: number;
  reason: string;
  status: MoneyRequestStatus;
  responseNote: string | null;
  respondedByName: string | null;
  createdAt: string;
  respondedAt: string | null;
}

export interface CreateMoneyRequestRequest {
  amount: number;
  reason: string;
}

export interface RespondToMoneyRequestRequest {
  approve: boolean;
  note?: string;
}

// Session DTOs
export interface SessionDto {
  id: string;
  ipAddress: string | null;
  userAgent: string | null;
  createdAt: string;
  expiresAt: string;
  isCurrent: boolean;
}

// Cashbook Types
export interface CashbookEntry {
  id: string;
  date: Date;
  description: string;
  category?: string;
  type: TransactionType;
  income?: number;
  expense?: number;
  runningBalance: number;
}

export interface CashbookSummary {
  month: string;
  openingBalance: number;
  totalIncome: number;
  totalExpenses: number;
  closingBalance: number;
}

// API Response wrapper
export interface ApiError {
  error: string;
}
