        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("GetToken")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjResponseApi))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ObjResponseApi))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ObjResponseApi))]
        public IActionResult GetToken([FromBody] GenerarToken req)
        {
            return ExceptionBehavior(() =>
            {
                bool AplicacionesExiste = RepoBusinessRules.Validaciones(req.ClientId, req.Cuenta);
                if (!AplicacionesExiste)
                    throw new CusException(EnumException.CusTypeException.Validation, EnumMessage.CusMessage.ErrorGeneral);


                var accessToken = new JwtTokenCreator().GenerateJwtToken(req);
                var refreshToken = new JwtTokenCreator().GenerateRefreshToken();
                return ResultApi(new { Token = accessToken, refreshToken });

            });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("Refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] RefreshToken req)
        {
            var principal = new JwtTokenCreator().GetPrincipalFromExpiredToken(req.Token);
            var username = principal.Identity.Name;
            //var savedRefreshToken = new JwtTokenCreator().GetRefreshToken(username);
            if ("savedRefreshToken" != req.refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            GenerarToken generar = new GenerarToken
            {
                Cuenta = "jgomemed",
                Correo = "jgomezm@intergrupo.com",
                ClientId = "7122df0c-55a8-448a-89af-7fa106a88f32"
            }; 
            var newJwtToken = new JwtTokenCreator().GenerateJwtToken(generar);
            var newRefreshToken = new JwtTokenCreator().GenerateRefreshToken();
            //new JwtTokenCreator().DeleteRefreshToken(username, refreshToken);
            //new JwtTokenCreator().SaveRefreshToken(username, newRefreshToken);

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
