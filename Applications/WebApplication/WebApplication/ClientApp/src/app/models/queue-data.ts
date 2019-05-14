export class QueueData {
    queue: string;
    value: number;
    measurement: string;
    lifetimeInMinutes: number;
    lifetimeInHours: number;
    lifetimeInDays: number;

    constructor(queue: string, value: number, measurement: string = 'N/A') {
        this.queue = queue;
        this.value = value;
        this.measurement = measurement;
    }
}
