export class Group {
    groupname: string;
    connections: IConnection[];
}
interface IConnection {
    connectionId: string;
    username: string;
}