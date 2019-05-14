
export class Chart {
    queueName: string;
    timeframe: number;
    dataset: any;
    measurement: string;

    constructor(queueName: string, timeframe: number, dataset: any = [], measurement: string = 'N/A') {
        this.queueName = queueName;
        this.timeframe = timeframe;
        this.dataset = dataset;
        this.measurement = measurement;
    }
}
