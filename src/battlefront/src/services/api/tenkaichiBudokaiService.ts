import axios, { AxiosInstance, AxiosResponse } from 'axios';

class TenkaichiBudokaiServiceService {
    private axiosInstance: AxiosInstance;

    constructor() {
        this.axiosInstance = axios.create({
            baseURL: 'https://tenkaichibudokai.bravemoss-a4238376.eastus2.azurecontainerapps.io/',
            headers: {
                'Content-Type': 'application/json',
            },
        });
    }

    public async initBattle(): Promise<AxiosResponse> {
        try {
            const response = await this.axiosInstance.post(`init-battle`, { healthPoints: 1000});
            return response;
        } catch (error) {
            throw error;
        }
    }
}

export default TenkaichiBudokaiServiceService;