import axios, { AxiosInstance, AxiosResponse } from 'axios';

export abstract class ApiService {
  private axiosInstance: AxiosInstance;

  constructor(baseURL: string) {
    this.axiosInstance = axios.create({
      baseURL: baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  public async get<T>(url: string, params?: any): Promise<AxiosResponse<T>> {
    const response = await this.axiosInstance.get<T>(url, { params });
    return response;
  }

  public async post<T>(url: string, data: any): Promise<AxiosResponse<T>> {
      const response = await this.axiosInstance.post<T>(url, data);
      return response;
  }

  public async put<T>(url: string, data: any): Promise<AxiosResponse<T>> {
      const response = await this.axiosInstance.put<T>(url, data);
      return response;
  }

  public async delete<T>(url: string): Promise<AxiosResponse<T>> {
      const response = await this.axiosInstance.delete<T>(url);
      return response;
  }

  public abstract attack(power: number): Promise<{ damage: number, healthpoints: number }>;
  public abstract getHealthPoints(): Promise<number>;
}

export default ApiService;