import axios, { AxiosInstance } from 'axios';
import config from '../config';

const apiAxiosInstance = axios.create({
  baseURL: config.apiUrl
});

class BaseApiService {
  private axiosInstance: AxiosInstance;

  constructor(axiosInstance: AxiosInstance = apiAxiosInstance) {
    this.axiosInstance = axiosInstance;
  }
}

export default BaseApiService;
