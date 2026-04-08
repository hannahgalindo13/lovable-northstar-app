const API_BASE = import.meta.env.VITE_API_URL;
const LARGE_PAGE_SIZE = 200;
const AUTH_TOKEN_KEY = "northstar-auth-token";

type WrappedResponse<T> = {
  success: boolean;
  data: T;
  message: string;
};

export class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

export type DashboardStats = {
  totalResidents: number;
  activeResidents: number;
  safehouseCount: number;
  totalDonations: number;
};

export type ImpactData = {
  headline: string;
  summary: string;
  metrics: {
    totalResidents?: number;
    activeResidents?: number;
    safehouseCount?: number;
    totalDonations?: number;
  };
  monthlyDonations: Array<{ month: string; amount: number }>;
  donationsGrowthPercent: number;
  programOutcomes: Array<{ program: string; rate: number }>;
  campaigns: Array<{ name: string; raised: number }>;
  allocationBreakdown: Array<{ label: string; amount: number; percent: number }>;
};

export type Supporter = {
  supporterId: number;
  supporterType: string;
  displayName: string;
  email?: string | null;
  status: string;
  createdAt: string;
};

export type Donation = {
  donationId: number;
  supporterId: number;
  donationType: string;
  donationDate: string;
  currencyCode?: string | null;
  amount?: number | null;
  estimatedValue?: number | null;
  impactUnit?: string | null;
  isRecurring?: number;
  campaignName?: string | null;
  notes?: string | null;
  channelSource: string;
};

export type DonorDonationHistoryItem = {
  donationId: number;
  amount?: number | null;
  estimatedValue?: number | null;
  donationDate: string;
  impactUnit?: string | null;
  donationType: string;
  notes?: string | null;
};

export type Resident = {
  residentId: number;
  caseControlNo: string;
  safehouseId: number;
  caseStatus: string;
  caseCategory: string;
  assignedSocialWorker?: string | null;
  currentRiskLevel: string;
  presentAge?: string | null;
  religion?: string | null;
  isPwd: number;
  hasSpecialNeeds: number;
  familySoloParent: number;
  familyIndigenous: number;
  dateOfAdmission: string;
  referralSource?: string | null;
  reintegrationStatus?: string | null;
  notesRestricted?: string | null;
  dateOfBirth?: string;
  sex?: string;
};

export type Safehouse = {
  safehouseId: number;
  name: string;
};

export type ProcessRecording = {
  recordingId: number;
  residentId: number;
  sessionDate: string;
  socialWorker: string;
  sessionType: string;
  emotionalStateObserved: string;
  sessionNarrative?: string | null;
  interventionsApplied?: string | null;
  followUpActions?: string | null;
};

export type HomeVisitation = {
  visitationId: number;
  residentId: number;
  visitDate: string;
  socialWorker: string;
  visitType: string;
  observations?: string | null;
  safetyConcernsNoted: number;
  followUpNotes?: string | null;
  followUpNeeded?: number;
  coordinationKind?: string;
};

export type AuthMeResponse = {
  isAuthenticated: boolean;
  email?: string;
  roles?: string[];
};

export type AuthResponse = {
  message: string;
  accessToken: string;
  email: string;
  roles: string[];
};

export const setAuthToken = (token: string) => {
  sessionStorage.setItem(AUTH_TOKEN_KEY, token);
};

export const clearAuthToken = () => {
  sessionStorage.removeItem(AUTH_TOKEN_KEY);
};

export const getAuthToken = () => sessionStorage.getItem(AUTH_TOKEN_KEY);

export type ResidentFormInput = {
  name: string;
  caseCategory: string;
  status: string;
  safehouseId: number;
  socialWorker: string;
};

async function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  try {
    const token = getAuthToken();
    const isPublicEndpoint = endpoint.startsWith("/public/");
    const mergedHeaders: HeadersInit = {
      "Content-Type": "application/json",
      ...(!isPublicEndpoint && token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options?.headers ?? {}),
    };

    const response = await fetch(`${API_BASE}/api${endpoint}`, {
      headers: mergedHeaders,
      ...options,
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("API ERROR:", errorText);
      throw new ApiError(response.status, `HTTP error: ${response.status}`);
    }

    const responseText = await response.text();
    if (!responseText) {
      return undefined as T;
    }
    return JSON.parse(responseText) as T;
  } catch (error) {
    console.error("API Error:", error);
    throw error;
  }
}

export const api = {
  get: <T>(endpoint: string) => request<T>(endpoint),
  post: <T>(endpoint: string, body: unknown) => {
    return request<T>(endpoint, {
      method: "POST",
      body: JSON.stringify(body),
    });
  },
  put: <T>(endpoint: string, body: unknown) => {
    return request<T>(endpoint, {
      method: "PUT",
      body: JSON.stringify(body),
    });
  },
  delete: <T>(endpoint: string) => request<T>(endpoint, { method: "DELETE" }),
};

const unwrap = <T>(value: WrappedResponse<T> | T): T => {
  if (typeof value === "object" && value !== null && "data" in value && "success" in value) {
    return (value as WrappedResponse<T>).data;
  }
  return value as T;
};

export const getDashboardStats = async () => {
  return api.get<DashboardStats>("/public/stats");
};

export const getImpactData = async () => {
  const response = await api.get<
    Omit<ImpactData, "metrics"> & {
      metrics: string | ImpactData["metrics"];
    }
  >("/public/impact");

  let parsedMetrics: ImpactData["metrics"] = {};
  if (typeof response.metrics === "string") {
    try {
      parsedMetrics = JSON.parse(response.metrics || "{}");
    } catch (error) {
      console.error("Invalid impact metrics JSON:", error);
    }
  } else {
    parsedMetrics = response.metrics;
  }

  return {
    ...response,
    metrics: parsedMetrics,
  } as ImpactData;
};

export const getSupporters = async () => {
  const response = await api.get<WrappedResponse<Supporter[]> | Supporter[]>(
    `/supporters?page=1&pageSize=${LARGE_PAGE_SIZE}`,
  );
  return unwrap(response);
};

export const createSupporter = async (
  payload: Pick<Supporter, "displayName" | "supporterType" | "status"> &
    Partial<Pick<Supporter, "email">> & {
      relationshipType: string;
    },
) => {
  const requestPayload = {
    supporterType: payload.supporterType,
    displayName: payload.displayName,
    organizationName: null,
    firstName: null,
    lastName: null,
    relationshipType: payload.relationshipType,
    region: null,
    country: null,
    email: payload.email ?? null,
    phone: null,
    status: payload.status,
    firstDonationDate: null,
    acquisitionChannel: null,
    createdAt: new Date().toISOString(),
  };
  const response = await api.post<WrappedResponse<Supporter> | Supporter>("/supporters", {
    ...requestPayload,
  });
  return unwrap(response);
};

export const getDonations = async () => {
  const response = await api.get<WrappedResponse<Donation[]> | Donation[]>(
    `/donations?page=1&pageSize=${LARGE_PAGE_SIZE}`,
  );
  return unwrap(response);
};

export const getMyDonations = async () => {
  const response = await api.get<WrappedResponse<DonorDonationHistoryItem[]> | DonorDonationHistoryItem[]>(
    "/donations/my",
  );
  return unwrap(response);
};

export const createDonation = async (
  payload: Omit<Donation, "donationId" | "supporterId"> & { supporterId?: number },
) => {
  const requestPayload = {
    supporterId: Number(payload.supporterId ?? 0),
    donationType: payload.donationType,
    donationDate: payload.donationDate,
    channelSource: payload.channelSource,
    currencyCode: payload.currencyCode ?? null,
    amount: payload.amount ?? null,
    estimatedValue: payload.estimatedValue ?? null,
    impactUnit: payload.impactUnit ?? null,
    isRecurring: Number(payload.isRecurring ?? 0),
    campaignName: payload.campaignName ?? null,
    notes: payload.notes ?? null,
    createdByPartnerId: null,
    referralPostId: null,
  };
  const response = await api.post<WrappedResponse<Donation> | Donation>("/donations", requestPayload);
  return unwrap(response);
};

export const updateDonation = async (id: number, payload: Donation) => {
  const requestPayload = {
    donationId: Number(payload.donationId),
    supporterId: Number(payload.supporterId),
    donationType: payload.donationType,
    donationDate: payload.donationDate,
    channelSource: payload.channelSource,
    currencyCode: payload.currencyCode ?? null,
    amount: payload.amount ?? null,
    estimatedValue: payload.estimatedValue ?? null,
    impactUnit: payload.impactUnit ?? null,
    isRecurring: Number(payload.isRecurring ?? 0),
    campaignName: payload.campaignName ?? null,
    notes: payload.notes ?? null,
    createdByPartnerId: null,
    referralPostId: null,
  };
  const response = await api.put<WrappedResponse<Donation> | Donation>(`/donations/${id}`, requestPayload);
  return unwrap(response);
};

export const deleteDonation = async (id: number) => {
  return api.delete(`/donations/${id}?confirm=true`);
};

export const getResidents = async () => {
  const response = await api.get<WrappedResponse<Resident[]> | Resident[]>(
    `/residents?page=1&pageSize=${LARGE_PAGE_SIZE}`,
  );
  return unwrap(response);
};

const toResidentPayload = (input: ResidentFormInput, existing?: Resident) => {
  const nowDate = new Date().toISOString().slice(0, 10);
  const createdAt = existing ? new Date().toISOString() : new Date().toISOString();
  const baseCaseNo = existing?.caseControlNo ?? `C${Date.now().toString().slice(-6)}`;
  return {
    residentId: existing?.residentId ?? 0,
    caseControlNo: baseCaseNo,
    internalCode: existing?.caseControlNo ?? `INT-${Date.now().toString().slice(-6)}`,
    safehouseId: input.safehouseId,
    caseStatus: input.status,
    sex: existing?.sex ?? "F",
    dateOfBirth: existing?.dateOfBirth ?? "2009-01-01",
    birthStatus: "Non-Marital",
    placeOfBirth: "Unknown",
    religion: existing?.religion ?? "Unknown",
    caseCategory: input.caseCategory,
    subCatOrphaned: 0,
    subCatTrafficked: 0,
    subCatChildLabor: 0,
    subCatPhysicalAbuse: 0,
    subCatSexualAbuse: 0,
    subCatOsaec: 0,
    subCatCicl: 0,
    subCatAtRisk: 1,
    subCatStreetChild: 0,
    subCatChildWithHiv: 0,
    isPwd: existing?.isPwd ?? 0,
    pwdType: null,
    hasSpecialNeeds: existing?.hasSpecialNeeds ?? 0,
    specialNeedsDiagnosis: null,
    familyIs4ps: 0,
    familySoloParent: existing?.familySoloParent ?? 0,
    familyIndigenous: existing?.familyIndigenous ?? 0,
    familyParentPwd: 0,
    familyInformalSettler: 0,
    dateOfAdmission: existing?.dateOfAdmission ?? nowDate,
    ageUponAdmission: null,
    presentAge: null,
    lengthOfStay: null,
    referralSource: existing?.referralSource ?? "Community",
    referringAgencyPerson: input.socialWorker,
    dateColbRegistered: null,
    dateColbObtained: null,
    assignedSocialWorker: input.socialWorker,
    initialCaseAssessment: "For monitoring",
    dateCaseStudyPrepared: null,
    reintegrationType: "None",
    reintegrationStatus: existing?.reintegrationStatus ?? "Not Started",
    initialRiskLevel: existing?.currentRiskLevel ?? "Medium",
    currentRiskLevel: existing?.currentRiskLevel ?? "Medium",
    dateEnrolled: existing?.dateOfAdmission ?? nowDate,
    dateClosed: null,
    createdAt,
    notesRestricted: existing?.notesRestricted ?? null,
  };
};

export const createResident = async (input: ResidentFormInput) => {
  const payload = toResidentPayload(input);
  const response = await api.post<WrappedResponse<Resident> | Resident>("/residents", payload);
  return unwrap(response);
};

export const updateResident = async (resident: Resident, input: ResidentFormInput) => {
  const payload = toResidentPayload(input, resident);
  const response = await api.put<WrappedResponse<Resident> | Resident>(`/residents/${resident.residentId}`, payload);
  return unwrap(response);
};

export const deleteResident = async (id: number) => {
  return api.delete(`/residents/${id}?confirm=true`);
};

export const getSafehouses = async () => {
  const response = await api.get<WrappedResponse<Safehouse[]> | Safehouse[]>(
    `/safehouses?page=1&pageSize=${LARGE_PAGE_SIZE}`,
  );
  return unwrap(response);
};

export const getProcessRecordings = async () => {
  const response = await api.get<WrappedResponse<ProcessRecording[]> | ProcessRecording[]>(
    `/processrecordings?page=1&pageSize=${LARGE_PAGE_SIZE}`,
  );
  return unwrap(response);
};

export const createProcessRecording = async (payload: Omit<ProcessRecording, "recordingId">) => {
  const requestPayload = {
    residentId: Number(payload.residentId),
    sessionDate: payload.sessionDate,
    socialWorker: payload.socialWorker,
    sessionType: payload.sessionType,
    sessionDurationMinutes: null,
    emotionalStateObserved: payload.emotionalStateObserved,
    emotionalStateEnd: null,
    sessionNarrative: payload.sessionNarrative ?? null,
    interventionsApplied: payload.interventionsApplied ?? null,
    followUpActions: payload.followUpActions ?? null,
    progressNoted: 0,
    concernsFlagged: 0,
    referralMade: 0,
    notesRestricted: null,
  };
  const response = await api.post<WrappedResponse<ProcessRecording> | ProcessRecording>(
    "/processrecordings",
    requestPayload,
  );
  return unwrap(response);
};

export const updateProcessRecording = async (id: number, payload: ProcessRecording) => {
  const requestPayload = {
    recordingId: Number(payload.recordingId),
    residentId: Number(payload.residentId),
    sessionDate: payload.sessionDate,
    socialWorker: payload.socialWorker,
    sessionType: payload.sessionType,
    sessionDurationMinutes: null,
    emotionalStateObserved: payload.emotionalStateObserved,
    emotionalStateEnd: null,
    sessionNarrative: payload.sessionNarrative ?? null,
    interventionsApplied: payload.interventionsApplied ?? null,
    followUpActions: payload.followUpActions ?? null,
    progressNoted: 0,
    concernsFlagged: 0,
    referralMade: 0,
    notesRestricted: null,
  };
  const response = await api.put<WrappedResponse<ProcessRecording> | ProcessRecording>(
    `/processrecordings/${id}`,
    requestPayload,
  );
  return unwrap(response);
};

export const deleteProcessRecording = async (id: number) => {
  return api.delete(`/processrecordings/${id}?confirm=true`);
};

export const getHomeVisitations = async () => {
  const response = await api.get<WrappedResponse<HomeVisitation[]> | HomeVisitation[]>(
    `/homevisitations?page=1&pageSize=${LARGE_PAGE_SIZE}`,
  );
  return unwrap(response);
};

export const createHomeVisitation = async (payload: Omit<HomeVisitation, "visitationId">) => {
  const requestPayload = {
    residentId: Number(payload.residentId),
    visitDate: payload.visitDate,
    socialWorker: payload.socialWorker,
    visitType: payload.visitType,
    locationVisited: null,
    familyMembersPresent: null,
    purpose: null,
    observations: payload.observations ?? null,
    familyCooperationLevel: null,
    safetyConcernsNoted: Number(payload.safetyConcernsNoted ?? 0),
    followUpNeeded: payload.followUpNotes ? 1 : 0,
    followUpNotes: payload.followUpNotes ?? null,
    visitOutcome: null,
    coordinationKind: "HomeVisit",
    visitTime: null,
  };
  const response = await api.post<WrappedResponse<HomeVisitation> | HomeVisitation>("/homevisitations", requestPayload);
  return unwrap(response);
};

export const updateSupporter = async (supporterId: number, payload: Supporter & { relationshipType: string }) => {
  const requestPayload = {
    supporterId: Number(payload.supporterId),
    supporterType: payload.supporterType,
    displayName: payload.displayName,
    organizationName: null,
    firstName: null,
    lastName: null,
    relationshipType: payload.relationshipType,
    region: null,
    country: null,
    email: payload.email ?? null,
    phone: null,
    status: payload.status,
    firstDonationDate: null,
    acquisitionChannel: null,
    createdAt: payload.createdAt,
  };
  const response = await api.put<WrappedResponse<Supporter> | Supporter>(`/supporters/${supporterId}`, requestPayload);
  return unwrap(response);
};

export const deleteSupporter = async (supporterId: number) => {
  return api.delete(`/supporters/${supporterId}?confirm=true`);
};

export const login = async (email: string, password: string, rememberMe = true) => {
  const response = await api.post<AuthResponse>("/auth/login", { email, password, rememberMe });
  setAuthToken(response.accessToken);
  return response;
};

export const register = async (email: string, password: string) => {
  const response = await api.post<AuthResponse>("/auth/register", { email, password });
  setAuthToken(response.accessToken);
  return response;
};

export const logout = async () => {
  try {
    await api.post("/auth/logout", {});
  } finally {
    clearAuthToken();
  }
};

export const getCurrentUser = async () => {
  return api.get<AuthMeResponse>("/auth/me");
};
