import { ApiService } from './apiService';

class FreezerService extends ApiService {
    private baseUrl: string = 'https://freezer.bravemoss-a4238376.eastus2.azurecontainerapps.io/';

    constructor() {
        super('');
    }

    async launchGenkiDama(energy: number): Promise<{ damage: number, healthpoints: number }> {
        const response = await this.post(`${this.baseUrl}launch-genki-dama`, { energy });

        return response.data as { damage: number, healthpoints: number };
    }

    async attack(energy: number): Promise<{ damage: number, healthpoints: number }> {
        const response = await this.post(`${this.baseUrl}attack-goku`, { energy });

        return response.data as { damage: number, healthpoints: number };
    }

    async getHealthPoints(): Promise<number> {
        const response = await this.get(`${this.baseUrl}health-points`);

        return response.data as number;
    }
}

export default FreezerService;