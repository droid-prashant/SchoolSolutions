export class JwtPayLoad {
    permission: string | string[] = "";
    role?: string | string[];
    roles?: string | string[];
    userId: string = "";
    username: string = "";
    sub: string = "";
    name: string = "";
    email: string = "";
    academicYear: string = "";
    exp: number = 0;
    iss: string = "";
    aud: string = "";
}
