import { apiClient } from './client';
import type {
  FamilyDto,
  FamilyMemberDto,
  ChildDto,
  CreateFamilyRequest,
  AddChildRequest,
  AddChildResponse,
  InviteMemberRequest,
  InvitationDto,
} from '../types';

export const familyApi = {
  async createFamily(data: CreateFamilyRequest): Promise<FamilyDto> {
    return apiClient.post<FamilyDto>('/family', data);
  },

  async getFamily(id: string): Promise<FamilyDto> {
    return apiClient.get<FamilyDto>(`/family/${id}`);
  },

  async getMyFamilies(): Promise<FamilyDto[]> {
    return apiClient.get<FamilyDto[]>('/family');
  },

  async getMembers(familyId: string): Promise<FamilyMemberDto[]> {
    return apiClient.get<FamilyMemberDto[]>(`/family/${familyId}/members`);
  },

  async getChildren(familyId: string): Promise<ChildDto[]> {
    return apiClient.get<ChildDto[]>(`/family/${familyId}/children`);
  },

  async addChild(familyId: string, data: AddChildRequest): Promise<AddChildResponse> {
    return apiClient.post<AddChildResponse>(`/family/${familyId}/children`, data);
  },

  async removeChild(familyId: string, childId: string): Promise<void> {
    await apiClient.delete(`/family/${familyId}/children/${childId}`);
  },

  async inviteMember(familyId: string, data: InviteMemberRequest): Promise<void> {
    await apiClient.post(`/family/${familyId}/invite`, data);
  },

  async getInvitations(): Promise<InvitationDto[]> {
    return apiClient.get<InvitationDto[]>('/family/invitations');
  },

  async acceptInvitation(invitationId: string): Promise<void> {
    await apiClient.post(`/family/invitations/${invitationId}/accept`);
  },

  async declineInvitation(invitationId: string): Promise<void> {
    await apiClient.post(`/family/invitations/${invitationId}/decline`);
  },

  async removeRelative(familyId: string, relativeId: string): Promise<void> {
    await apiClient.delete(`/family/${familyId}/relatives/${relativeId}`);
  },
};
