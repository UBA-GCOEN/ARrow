import userModel from "../models/userModel.js"
import bcrypt from 'bcrypt'


/**
 * Route: /getDeletePage
 * Desc: get the details of 
 *       current user before 
 *       deleting the user
 */
export const getDeletePage = async (req, res) => {
    var email = req.session.user.user.email
    var role = req.role
    var name = req.session.user.user.name

    res.json({
        name,
        email,
        role
    })
}





/**
 * Route: /deleteUser
 * Desc: Delete the current 
 *       logged in user
 */
export const deleteUser = async (req, res) => {
 
    var { email, name, password } = req.body

    var role = req.role

    const emailDomains = [
        "@gmail.com",
        "@yahoo.com",
        "@hotmail.com",
        "@aol.com",
        "@outlook.com",
        "@gcoen.ac.in",
        ];

 
 
        //check name length
        if (name.length < 2) {
          return res
            .status(404)
            .json({ message: "Name must be atleast 2 characters long." });
        }
 
        // check email format
        if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
          return res.status(404).json({
         message: "Please enter a valid email address",
       })};



       /**
        * checking field types
        * to avoid sql attacks
        */
       if (typeof name !== "string") {
        res.status(400).json({ status: "error"+name+ typeof name });
        return;
      }

      if (typeof email !== "string") {
        res.status(400).json({ status: "error"+email+typeof email });
        return;
      }
      
      




        // conditions to figure out role
        if(role === 'admin'){
                
            var oldUser = await userAdminModel.findOne({email})
            var isPasswordCorrect = bcrypt.compare(oldUser.password, password)
            
            if(isPasswordCorrect) {
                oldUser = await userAdminModel.deleteOne({email})
                res.send("User deleted successffuly")
            }
            else{
                res.send("Incorrect Password")
            }

        }
        else if(role === 'student'){


            var oldUser = await userStudentModel.findOne({email})
            var isPasswordCorrect = bcrypt.compare(oldUser.password, password)
            
            if(isPasswordCorrect) {
                oldUser = await userStudentModel.deleteOne({email})
                res.send("User deleted successffuly")
            }
            else{
                res.send("Incorrect Password")
            }
        }
        else if(role === 'staff'){
            
            var oldUser = await userStaffModel.findOne({email})
            var isPasswordCorrect = bcrypt.compare(oldUser.password, password)
            
            if(isPasswordCorrect) {
                oldUser = await userStaffModel.deleteOne({email})
                res.send("User deleted successffuly")
            }
            else{
                res.send("Incorrect Password")
            }
        }
        else if(role === 'faculty'){

            var oldUser = await userFacultyModel.findOne({email})
            var isPasswordCorrect = bcrypt.compare(oldUser.password, password)
            
            if(isPasswordCorrect) {
                oldUser = await userFacultyModel.deleteOne({email})
                res.send("User deleted successffuly")
            }
            else{
                res.send("Incorrect Password")
            }
        }
        else if(role == 'visitor'){
            
            var oldUser = await userVisitorModel.findOne({email})
            var isPasswordCorrect = bcrypt.compare(oldUser.password, password)
            
            if(isPasswordCorrect) {
                oldUser = await userVisitorModel.deleteOne({email})
                res.send("User deleted successffuly")
            }
            else{
                res.send("Incorrect Password")
            }
        }

    
}