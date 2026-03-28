export interface Member {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  department: string;
  position: string;
  nationality: string;
  dateOfBirth: string;
  passportNumber: string;
  avatarUrl: string | null;
  documentCompletionPercent: number;
  status: 'Active' | 'Inactive' | 'Suspended';
  createdAt: string;
  updatedAt: string;
}

export interface MemberDetail extends Member {
  address: string;
  city: string;
  country: string;
  emergencyContactName: string;
  emergencyContactPhone: string;
  notes: string;
  documents: MemberDocument[];
  travelHistory: TravelRecord[];
}

export interface MemberDocument {
  id: string;
  title: string;
  type: string;
  category: string;
  status: string;
  expiryDate: string | null;
  uploadedAt: string;
}

export interface TravelRecord {
  id: string;
  caseName: string;
  destination: string;
  departureDate: string;
  returnDate: string;
  status: string;
}
