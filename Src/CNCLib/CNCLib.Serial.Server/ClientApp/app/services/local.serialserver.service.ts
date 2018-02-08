import { Injectable, Inject } from '@angular/core';
import { Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { SerialServerService } from './serialserver.service';
import { SerialCommand } from "../models/serial.command";
import { SerialPortDefinition } from '../models/serial.port.definition';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'

@Injectable()
export class LocalSerialServerService implements SerialServerService 
{
    constructor(
        private http: HttpClient,
        @Inject('BASE_URL') public baseUrl: string,
    ) 
    {
    }

    getInfo(): Promise<CNCLibServerInfo>
    {
        return this.http.get<CNCLibServerInfo>(this.baseUrl + 'api/Info').toPromise()
            .catch(this.handleErrorPromise);
    }

    getPorts(): Promise<SerialPortDefinition[]> 
    {
        return this.http.get<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort').toPromise()
            .catch(this.handleErrorPromise);
    }

    getPort(id: number): Promise<SerialPortDefinition>
    {
        return this.http.get<SerialPortDefinition>(this.baseUrl + 'api/SerialPort/' + id).toPromise();
    }

    refresh(): Promise<SerialPortDefinition[]> 
    {
        return this.http.post<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort',"x").toPromise()
            .catch(this.handleErrorPromise);
    }

    connect(serialportid: number, baudrate: number, resetonConnect: boolean): Promise<void>
    {
        return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/connect/?baudrate=' + baudrate, "x").toPromise()
            .catch(this.handleErrorPromise);
    }

    disconnect(serialportid: number): Promise<void>
    {
        return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/disconnect', "x").toPromise()
            .catch(this.handleErrorPromise);
    }

    abort(serialportid: number): Promise<void>
    {
        return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/abort', "x").toPromise()
            .catch(this.handleErrorPromise);
    }

    resume(serialportid: number): Promise<void>
    {
        return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/resume', "x").toPromise()
            .catch(this.handleErrorPromise);
    }

    getHistory(serialportid: number): Promise<SerialCommand[]> 
    {
        return this.http.get<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/history').toPromise()
            .catch(this.handleErrorPromise);
    }

    clearHistory(serialportid: number): Promise<void>
    {
        return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/history/clear', "x").toPromise()
            .catch(this.handleErrorPromise);
    }

    private handleErrorPromise(error: Response | any)
    {
        console.error(error.message || error);
        return Promise.reject(error.message || error);
    }	
}
