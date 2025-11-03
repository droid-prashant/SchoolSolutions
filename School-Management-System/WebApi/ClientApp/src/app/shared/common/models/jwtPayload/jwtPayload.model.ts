export class JwtPayLoad {
    permission: string = "";
    sub: string = "";
    name: string = "";
    email: string = "";
    exp: number = 0;
    iss: string = "";
    aud: string = "";
}